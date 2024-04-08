using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using Skills.Databases;
using Skills.Models;
using Skills.Models.Enums;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Skills.Services;

public class SkillService(IConfiguration configuration, IDbContextFactory<SkillsContext> factory)
{
    public async Task InitAsync()
    {
        var db = await factory.CreateDbContextAsync();
        var softSkillExists = db.SkillsTypes.AsNoTracking()
            .FirstOrDefault(x => x.Type == SkillDataType.Type && x.Value == "SOFT-SKILL") != null;
        if (!softSkillExists)
        {
            db.SkillsTypes.Add(new SKillInfo { Type = SkillDataType.Type, Value = "SOFT-SKILL" });
            await db.SaveChangesAsync();
        }

        await db.DisposeAsync();
    }

    public async Task<KeyValuePair<ImportState, string>> ImportXlsxAsync(Stream stream, string startCell, ImportType importType = ImportType.Purge)
    {
        var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        ms.Position = 0;

        await stream.DisposeAsync();

        var excelFile = await ms.QueryAsync<SkillRowModel>(null, ExcelType.XLSX, startCell);
        if (excelFile is null) return new KeyValuePair<ImportState, string>(ImportState.Cancelled, "Le fichier n'a pas pu être importé à cause d'un mauvais format ou le nom des colonnes n'est pas respecté.");

        switch (importType)
        {
            case ImportType.Purge:
                return await PurgeSkillsAsync(excelFile);
                break;
        }
        await ms.DisposeAsync(); // If we dispose it earlier we won't be able to enumerate over the excelfile rows

        return new KeyValuePair<ImportState, string>(ImportState.Skipped, "Aucun type d'import n'a été sélectionné par défaut ou celui sélectionné n'est pas supporté par l'application ! Veuillez contacter l'équipe de développement.");
    }

    private async Task<KeyValuePair<ImportState, string>> PurgeSkillsAsync(IEnumerable<SkillRowModel> skills)
    {
        try
        {
            await PurgeAllAsync();
            var db = await factory.CreateDbContextAsync();

            var index = 1;
            foreach (var row in skills)
            {
                db = await factory.CreateDbContextAsync();
                index++;
                if (row.Type.Equals("SOFT-SKILL", StringComparison.OrdinalIgnoreCase))
                {
                    // SoftSkill Model
                    if (string.IsNullOrWhiteSpace(row.Description)) return new KeyValuePair<ImportState, string>(ImportState.Cancelled, $"Un Soft-Skill se doit d'avoir au moins une description ! (~~ligne: {index})");
                    var softType = db.SkillsTypes.AsNoTracking().First(x => x.Type == SkillDataType.Type && x.Value == "SOFT-SKILL");
                    var softSkill = new SoftSkillModel
                    {
                        TypeId = softType.Id,
                        Type = softType.Value,
                        Description = row.Description
                    };

                    db.SoftSkills.Add(softSkill);
                    await db.SaveChangesAsync();

                    // SoftType Levels
                    if (!string.IsNullOrWhiteSpace(row.Lvl1)) db.SoftTypesLevels.Add(new SoftTypeLevel { SkillId = softSkill.Id, Value = row.Lvl1, Level = 1 });
                    if (!string.IsNullOrWhiteSpace(row.Lvl2)) db.SoftTypesLevels.Add(new SoftTypeLevel { SkillId = softSkill.Id, Value = row.Lvl2, Level = 2 });
                    if (!string.IsNullOrWhiteSpace(row.Lvl3)) db.SoftTypesLevels.Add(new SoftTypeLevel { SkillId = softSkill.Id, Value = row.Lvl3, Level = 3 });
                    if (!string.IsNullOrWhiteSpace(row.Lvl4)) db.SoftTypesLevels.Add(new SoftTypeLevel { SkillId = softSkill.Id, Value = row.Lvl4, Level = 4 });
                    await db.SaveChangesAsync();
                }
                else
                {
                    // Skill Types
                    var oldType = db.SkillsTypes.AsNoTracking().FirstOrDefault(x => x.Type == SkillDataType.Type && x.Value == row.Type);
                    var oldCategory = db.SkillsTypes.AsNoTracking().FirstOrDefault(x => x.Type == SkillDataType.Category && x.Value == row.Category);
                    var oldSubCategory = db.SkillsTypes.AsNoTracking().FirstOrDefault(x => x.Type == SkillDataType.SubCategory && x.Value == row.SubCategory);

                    // Skill Model
                    var type = oldType;
                    var skill = new SkillModel { Description = row.Description };
                    if (!string.IsNullOrWhiteSpace(row.Type))
                    {
                        if (oldType != null)
                        {
                            oldType.Value = row.Type;
                            db.SkillsTypes.Update(oldType);
                            skill.TypeId = oldType.Id;
                            skill.Type = oldType.Value;
                        }
                        else
                        {
                            type = new SKillInfo { Type = SkillDataType.Type, Value = row.Type };
                            db.SkillsTypes.Add(type);
                            skill.TypeId = type.Id;
                            skill.Type = type.Value;
                        }
                    }
                    
                    if (!string.IsNullOrWhiteSpace(row.Category))
                    {
                        if (oldCategory != null)
                        {
                            oldCategory.Value = row.Category;
                            db.SkillsTypes.Update(oldCategory);
                            skill.CategoryId = oldCategory.Id;
                            skill.Category = oldCategory.Value;
                        }
                        else
                        {
                            var category = new SKillInfo { Type = SkillDataType.Category, Value = row.Category };
                            db.SkillsTypes.Add(category);
                            skill.CategoryId = category.Id;
                            skill.Category = category.Value;
                        }
                    }
                    
                    if (!string.IsNullOrWhiteSpace(row.SubCategory))
                    {
                        if (oldSubCategory != null)
                        {
                            oldSubCategory.Value = row.SubCategory;
                            db.SkillsTypes.Update(oldSubCategory);
                            skill.CategoryId = oldSubCategory.Id;
                            skill.Category = oldSubCategory.Value;
                        }
                        else
                        {
                            var subcategory = new SKillInfo { Type = SkillDataType.SubCategory, Value = row.SubCategory };
                            db.SkillsTypes.Add(subcategory);
                            skill.SubCategoryId = subcategory.Id;
                            skill.SubCategory = subcategory.Value;
                        }
                    }

                    await db.SaveChangesAsync(); // Skill Types must be saved before being used as foreign keys in a SkillModel

                    db.Skills.Add(skill);
                    await db.SaveChangesAsync();

                    // If the type isn't specified but at least one level is present : levels depends on the type therefore it can't be null
                    if (type is null && (!string.IsNullOrWhiteSpace(row.Lvl1) || !string.IsNullOrWhiteSpace(row.Lvl2) ||
                                         !string.IsNullOrWhiteSpace(row.Lvl3) || !string.IsNullOrWhiteSpace(row.Lvl4)))
                    {
                        return new KeyValuePair<ImportState, string>(ImportState.Cancelled, $"Les niveaux dépendent des types pour les compétences classiques (hors soft-skill). (~~ligne: {index})");
                    }

                    if (type is null) continue;

                    var levelIndex = 1;
                    foreach (var level in new[] { row.Lvl1, row.Lvl2, row.Lvl3, row.Lvl4 })
                    {
                        var oldLevel = db.TypesLevels.AsNoTracking().FirstOrDefault(x => x.TypeId == type.Id && x.Level == levelIndex);
                        if (!string.IsNullOrWhiteSpace(level) && oldLevel != null && level != oldLevel.Value)
                        {
                            oldLevel.Value = level;
                            db.TypesLevels.Update(oldLevel);
                        }
                        else if (!string.IsNullOrWhiteSpace(level))
                        {
                            db.TypesLevels.Add(new TypeLevel
                            {
                                TypeId = type!.Id,
                                Level = levelIndex,
                                Value = level
                            });
                        }

                        await db.SaveChangesAsync();
                        levelIndex++;
                    }
                }

                await db.DisposeAsync();
            }

            return new KeyValuePair<ImportState, string>(ImportState.Successful, $"Toutes les compétences ont été importées avec succès ! ({index} importées)");
        }
        catch (Exception e)
        {
            return new KeyValuePair<ImportState, string>(ImportState.Crashed, $"Une exception a été levée pendant le traitement, veuillez communiquer cette erreur à l'équipe de déleloppement : {e.Message}");
        }
    }

    public async Task PurgeAllAsync()
    {
        var db = await factory.CreateDbContextAsync();
        var savedTypes = db.SkillsTypes.AsNoTracking().ToList();
        var savedSkills = db.Skills.AsNoTracking().ToList();
        db.RemoveRange(savedTypes);
        db.RemoveRange(savedSkills);
        await db.SaveChangesAsync();
        await db.DisposeAsync();
        await InitAsync(); // Add back the Soft-Skill type (hardcoded skill type, required for the global app)
    }

    /// <summary>
    /// Class used for xlsx deserialization.
    /// </summary>
    private sealed class SkillRowModel
    {
        [ExcelColumn(Name = "TYPE")] public string Type { get; set; } = string.Empty;
        [ExcelColumn(Name = "CATEGORIE")] public string Category { get; set; } = string.Empty;
        [ExcelColumn(Name = "SS-CATEGORIE")] public string SubCategory { get; set; } = string.Empty;
        [ExcelColumn(Name = "DESCRIPTION")] public string Description { get; set; } = string.Empty;
        [ExcelColumn(Name = "Niveau 1")] public string Lvl1 { get; set; } = string.Empty;
        [ExcelColumn(Name = "Niveau 2")] public string Lvl2 { get; set; } = string.Empty;
        [ExcelColumn(Name = "Niveau 3")] public string Lvl3 { get; set; } = string.Empty;
        [ExcelColumn(Name = "Niveau 4")] public string Lvl4 { get; set; } = string.Empty;
    }
}