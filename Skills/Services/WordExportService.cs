using DocumentFormat.OpenXml.Packaging;
using Microsoft.EntityFrameworkCore;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Models.CV;
using Skills.Models.Enums;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Path = System.IO.Path;

namespace Skills.Services;

public class WordExportService(IConfiguration configuration, IDbContextFactory<SkillsContext> factory)
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
    
    /// <summary>
    /// Export the given CV into a stream.
    /// </summary>
    /// <param name="cv">The CV to export.</param>
    /// <returns>A stream representing the cv as a Word document.</returns>
    public async Task<TransactionResult<T>> ExportCvAsync<T>(CvInfo cv) where T : Stream
    {
        FileStream? fs = null;
        string tempFile = string.Empty;
        
        try
        {
            await _semaphore.WaitAsync();
            
            var fileName = configuration.GetSection("Export")["WordTemplateFile"] ?? string.Empty;
            var fileFolder = configuration.GetSection("Export")["WordTemplateFolder"] ?? string.Empty;
            var dir = Directory.GetCurrentDirectory();
            var template = Path.Combine(dir, fileFolder, fileName);
            tempFile = Path.Combine(dir, configuration.GetSection("Export")["WordTemporaryFolder"] ?? string.Empty, "temp.docx");
            
            Console.WriteLine($"Template PATH -> {template}");
            Console.WriteLine($"TempFile PATH -> {tempFile}");
            
            if (File.Exists(tempFile)) File.Delete(tempFile);
            File.Copy(template, tempFile);
            
            fs = File.Open(tempFile, FileMode.Open);

            var db = await factory.CreateDbContextAsync();
            var user = db.Users.AsNoTracking().FirstOrDefault(x => x.Id == cv.UserId);
            if (user is null)
            {
                return new TransactionResult<T>
                {
                    State = ImportState.Skipped,
                    Message = "L'utilisateur spécifié est introuvable dans la base !",
                    Value = null
                };
            }
            await db.DisposeAsync();
            var document = WordprocessingDocument.Open(fs, true);

            SetHeader(cv, user, document);
            InsertEducation(cv, document);
            InsertCertifications(cv, document);
            InsertSafety(cv, document);
            InsertSkills(cv, document);
            InsertExperiences(cv, document);
            
            document.Save();
            document.Dispose();

            fs.Seek(0, SeekOrigin.Begin);
            var ms = new MemoryStream();
            await fs.CopyToAsync(ms);
            ms.Position = 0;
            return new TransactionResult<T>
            {
                State = ImportState.Successful,
                Value = ms as T
            };
        }
        catch (Exception e)
        {
            return new TransactionResult<T>
            {
                State = ImportState.Crashed,
                Message = "Une erreur est survenue lors de la génération du CV !",
                ErrorMessage = e.Message,
                 Value = null
            };
        }
        finally
        {
            _semaphore.Release(1);
            if (fs != null) await fs.DisposeAsync();
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    /// <summary>
    /// Sets the header of the first page (contact, trigram, job, ...)
    /// </summary>
    /// <param name="cv">The CV to export</param>
    /// <param name="user">The user of the CV</param>
    /// <param name="document">The Word document</param>
    private void SetHeader(CvInfo cv, UserModel user, WordprocessingDocument document)
    {
        document.SearchAndReplaceInTables("{TRIGRAMME}",user.GetTrigramme());
        document.SearchAndReplaceInTables("{JOB}", cv.Job);
        document.SearchAndReplaceInTables("{YEAR}", cv.BirthDate.Year.ToString());
        document.SearchAndReplaceInTables("{PHONE_NUMBER}", cv.PhoneNumber);
    }

    private void InsertEducation(CvInfo cv, WordprocessingDocument document)
    {
        var reference = document.Search<Paragraph>("{EDUCATION}");
        if (reference is null) return;
        
        foreach (var education in cv.Educations)
        {
            var firstLine = DocXtensions.CreateTitle($"{education.Title} | ", DocXtensions.HeadingLevel.H2);
            firstLine.Append(DocXtensions.CreateRunAsTitle(education.Supplier, DocXtensions.HeadingLevel.Accent));
            reference.InsertBeforeSelf(firstLine);

            var secondLine = DocXtensions.CreateTitle(education.YearStart.ToString(), DocXtensions.HeadingLevel.H3);
            secondLine.Append(DocXtensions.CreateRunAsTitle(" - ", DocXtensions.HeadingLevel.H3));
            secondLine.Append(DocXtensions.CreateRunAsTitle(education.YearEnd.ToString(), DocXtensions.HeadingLevel.H3));
            secondLine.Append(DocXtensions.CreateRunAsTitle(" | ", DocXtensions.HeadingLevel.H3));
            secondLine.Append(DocXtensions.CreateRunAsTitle(education.Town, DocXtensions.HeadingLevel.H3));
            reference.InsertBeforeSelf(secondLine);

            if (!string.IsNullOrWhiteSpace(education.Description))
            {
                var lines = education.Description.Split("\n");
                
                var listCreated = false;
                int listId = 0;
                foreach (var line in lines)
                {
                    if (line.StartsWith("- ") || line.StartsWith(" - "))
                    {
                        if (!listCreated)
                        {
                            listCreated = true;
                            listId = document.MainDocumentPart!.AddBulletList();
                            reference.InsertBeforeSelf(DocXtensions.ConvertRunToBulletList(
                                DocXtensions.CreateRunAsTitle(line.Replace(" - ", string.Empty).Replace("- ", string.Empty), DocXtensions.HeadingLevel.Normal), listId));
                        }
                        else
                        {
                            reference.InsertBeforeSelf(DocXtensions.ConvertRunToBulletList(
                                DocXtensions.CreateRunAsTitle(line.Replace(" - ", string.Empty).Replace("- ", string.Empty), DocXtensions.HeadingLevel.Normal), listId));
                        }
                    }
                    else
                    {
                        listCreated = false;
                        reference.InsertBeforeSelf(DocXtensions.CreateTitle(line, DocXtensions.HeadingLevel.Normal));
                    }
                }
            }
        }
        
        reference.Remove();
    }

    private void InsertCertifications(CvInfo cv, WordprocessingDocument document)
    {
        var reference = document.Search<Paragraph>("{CERTIFICATIONS}");
        if (reference is null) return;

        foreach (var certification in cv.Certifications)
        {
            var firstLine = DocXtensions.CreateTitle($"{certification.Title} | ", DocXtensions.HeadingLevel.H2);
            firstLine.Append(DocXtensions.CreateRunAsTitle(certification.Supplier, DocXtensions.HeadingLevel.Accent));
            reference.InsertBeforeSelf(firstLine);
            
            var secondLine = DocXtensions.CreateTitle(certification.Year.ToString(), DocXtensions.HeadingLevel.H3);
            secondLine.Append(DocXtensions.CreateRunAsTitle(" - ", DocXtensions.HeadingLevel.H3));
            secondLine.Append(DocXtensions.CreateRunAsTitle(certification.Duration, DocXtensions.HeadingLevel.H3));
            reference.InsertBeforeSelf(secondLine);
        }
        
        reference.Remove();
    }

    private void InsertSafety(CvInfo cv, WordprocessingDocument document)
    {
        var reference = document.Search<Paragraph>("{SAFETY}");
        if (reference is null) return;

        var certifications = cv.SafetyCertifications.Where(x => x.Certification != null).Select(x => x.Certification);
        var groups = certifications.GroupBy(x => x!.Category).ToDictionary(x => x.Key, y => y.ToList());
        foreach (var safety in groups.Keys)
        {
            var line = DocXtensions.CreateTitle($"{safety} | ", DocXtensions.HeadingLevel.H2);
            line.Append(DocXtensions.CreateRunAsTitle(string.Join(", ", groups[safety].Select(x => x?.Name ?? string.Empty)), DocXtensions.HeadingLevel.Accent));
            reference.InsertBeforeSelf(line);
        }
        
        reference.Remove();
    }

    private void InsertSkills(CvInfo cv, WordprocessingDocument document)
    {
        var reference = document.Search<Paragraph>("{SKILLS}");
        if (reference is null) return;

        var skillTypes = cv.Skills.GroupBy(x => x.Skill!.Type ?? string.Empty).ToDictionary(x => x.Key, x => x.ToList().Select(y => y.Skill));
        foreach (var type in skillTypes.Keys)
        {
            foreach (var category in skillTypes[type].Where(x => !string.IsNullOrWhiteSpace(x!.Category)).Select(x => x?.Category ?? string.Empty).Distinct().OrderDescending())
            {
                var categoryLine = DocXtensions.CreateTitle(category ?? string.Empty, DocXtensions.HeadingLevel.H2);
                reference.InsertBeforeSelf(categoryLine);
                
                if (type.ToLower() == "soft-skill")
                {
                    var skillsLine = DocXtensions.CreateTitle(string.Empty, DocXtensions.HeadingLevel.H4);
                    skillsLine.Append(DocXtensions.CreateRunAsTitle(string.Join(" ; ", skillTypes[type].Select(x => x?.Description ?? string.Empty)), DocXtensions.HeadingLevel.Accent));
                    reference.InsertBeforeSelf(skillsLine);
                }
                else
                {
                    foreach (var subCategory in skillTypes[type].Where(x => x!.Category == category && !string.IsNullOrWhiteSpace(x!.SubCategory)).Select(x => x!.SubCategory).Distinct().OrderDescending())
                    {
                        var skillsLine = DocXtensions.CreateTitle($"{subCategory}: ", DocXtensions.HeadingLevel.H4);
                        skillsLine.Append(DocXtensions.CreateRunAsTitle(string.Join(" ; ", skillTypes[type].Where(x => x?.SubCategory == subCategory).Select(x => x?.Description ?? string.Empty)), DocXtensions.HeadingLevel.Accent));
                        reference.InsertBeforeSelf(skillsLine);
                    }
                
                    var uncategorizedSkills = skillTypes[type].Where(x => string.IsNullOrWhiteSpace(x!.Category)).ToList();
                    var lines = DocXtensions.CreateParagraph(string.Empty);
                    lines.Append(DocXtensions.CreateRunAsTitle(string.Join(" ; ", uncategorizedSkills.Select(x => x?.Description ?? string.Empty)), DocXtensions.HeadingLevel.Accent));
                    reference.InsertBeforeSelf(lines);
                }
            }
        }
        
        reference.Remove();
    }

    private void InsertExperiences(CvInfo cv, WordprocessingDocument document)
    {
        var reference = document.Search<Paragraph>("{EXPERIENCES}");
        if (reference is null) return;
        
        foreach (var experience in cv.Experiences)
        {
            var firstLine = DocXtensions.CreateTitle($"{experience.Category} | ", DocXtensions.HeadingLevel.H2);
            firstLine.Append(DocXtensions.CreateRunAsTitle(experience.Title, DocXtensions.HeadingLevel.Accent));
            reference.InsertBeforeSelf(firstLine);
            
            var secondLine = DocXtensions.CreateTitle(experience.StartsAt.ToString("MM-yyyy"), DocXtensions.HeadingLevel.H3);
            secondLine.Append(DocXtensions.CreateRunAsTitle(" - ", DocXtensions.HeadingLevel.H3));
            secondLine.Append(DocXtensions.CreateRunAsTitle(experience.EndsAt.ToString("MM-yyyy"), DocXtensions.HeadingLevel.H3));
            reference.InsertBeforeSelf(secondLine);

            if (!string.IsNullOrWhiteSpace(experience.Description))
            {
                var lines = experience.Description.Split("\n");

                var listCreated = false;
                int listId = 0;
                foreach (var line in lines)
                {
                    if (line.StartsWith("- ") || line.StartsWith(" - "))
                    {
                        if (!listCreated)
                        {
                            listCreated = true;
                            listId = document.MainDocumentPart!.AddBulletList();
                            reference.InsertBeforeSelf(DocXtensions.ConvertRunToBulletList(
                                DocXtensions.CreateRunAsTitle(line.Replace(" - ", string.Empty).Replace("- ", string.Empty), DocXtensions.HeadingLevel.Normal), listId));
                        }
                        else
                        {
                            reference.InsertBeforeSelf(DocXtensions.ConvertRunToBulletList(
                                DocXtensions.CreateRunAsTitle(line.Replace(" - ", string.Empty).Replace("- ", string.Empty), DocXtensions.HeadingLevel.Normal), listId));
                        }
                    }
                    else
                    {
                        listCreated = false;
                        reference.InsertBeforeSelf(DocXtensions.CreateTitle(line, DocXtensions.HeadingLevel.Normal));
                    }
                }
            }

            reference.InsertBeforeSelf(DocXtensions.AddLineBreak());
        }
        
        reference.Remove();
    }
}

