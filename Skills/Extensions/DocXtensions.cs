using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using DocumentFormat.OpenXml.Wordprocessing;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace Skills.Extensions;

public static class DocXtensions
{
    public static Paragraph CreateParagraph(string text) => new Paragraph(new Run(new Text(text)));
    
    public static Paragraph CreateTitle(string content, HeadingLevel level)
    {
        var paragraph = new Paragraph(GetParagraphProperties(level));
        var text = new Text()
        {
            Text = content,
            Space = SpaceProcessingModeValues.Preserve
        };
        paragraph.Append(new Run(text));
        return paragraph;
    }
    
    public static Run CreateRunAsTitle(string content, HeadingLevel level)
    {
        var run = new Run(GetRunProperties(level));
        var text = new Text()
        {
            Text = content,
            Space = SpaceProcessingModeValues.Preserve
        };
        run.Append(new Run(text));
        return run;
    }

    public static Paragraph AddLineBreak() => new Paragraph();

    public static Paragraph AddPageBreak() => new Paragraph(new Break() { Type = BreakValues.Page });

    public static int AddBulletList(this MainDocumentPart mainPart)
    {
        var numberingPart = mainPart.NumberingDefinitionsPart;
        if (numberingPart == null)
        {
            numberingPart = mainPart.AddNewPart<NumberingDefinitionsPart>(Guid.NewGuid().ToString());
            Numbering element = new Numbering();
            element.Save(numberingPart);
        }
        
        var abstractNumberId = numberingPart.Numbering.Elements<AbstractNum>().Count() + 1;
        var abstractLevel = new Level(new NumberingFormat() { Val = NumberFormatValues.Bullet }, new LevelText() { Val = "-" }) { LevelIndex = 0 };
        var abstractNum = new AbstractNum(abstractLevel) { AbstractNumberId = abstractNumberId };

        if (abstractNumberId == 1)
        {
            numberingPart.Numbering.Append(abstractNum);
        }
        else
        {
            var lastAbstractNum = numberingPart.Numbering.Elements<AbstractNum>().Last();
            numberingPart.Numbering.InsertAfter(abstractNum, lastAbstractNum);
        }

        var numberId = numberingPart.Numbering.Elements<NumberingInstance>().Count() + 1;
        var numberingInstance1 = new NumberingInstance() { NumberID = numberId };
        var abstractNumId1 = new AbstractNumId() { Val = abstractNumberId };
        numberingInstance1.Append(abstractNumId1);

        if (numberId == 1)
        {
            numberingPart.Numbering.Append(numberingInstance1);
        }
        else
        {
            var lastNumberingInstance = numberingPart.Numbering.Elements<NumberingInstance>().Last();
            numberingPart.Numbering.InsertAfter(numberingInstance1, lastNumberingInstance);
        }

        return numberId;
    }
    
    public static Paragraph ConvertRunToBulletList(Run run, int pNumberId)
    {
        var numberingProperties = new NumberingProperties(new NumberingLevelReference() { Val = 0 }, new NumberingId() { Val = pNumberId });
        var indentation = new Indentation { Left = "720", Hanging = "360" };

        var paragraphMarkRunProperties = new ParagraphMarkRunProperties();
        paragraphMarkRunProperties.Append(new RunFonts { Ascii = "Symbol", HighAnsi = "Symbol" });
        
        var paragraphProperties = new ParagraphProperties(numberingProperties, indentation, paragraphMarkRunProperties);
        
        var newPara = new Paragraph(paragraphProperties);
        newPara.AppendChild(run);

        return newPara;
    }

    
    
    /// <summary>
    /// Creates a paragraph with the given texts and properties. Each text has its own properties that will be only applied to itself.
    /// </summary>
    /// <param name="paragraphProperties">An array of OpenXmlElement that defines the returned paragraph properties.</param>
    /// <param name="runs">An array of  that defines for each string a run with its properties.</param>
    /// <returns>A fully customized paragraph</returns>
    public static Paragraph CreateComplexParagraph(OpenXmlElement[]? paragraphProperties = null, params KeyValuePair<string, OpenXmlElement[]?>[] runs)
    {
        var para = new Paragraph();
        if (paragraphProperties is not null)
            para.AppendChild(new ParagraphProperties(paragraphProperties));

        foreach (var key in runs)
        {
            if (key.Value is null || key.Value.Count() == 0)
            {
                para.AppendChild(new Run(new Text(key.Key)));
            }
            else
            {
                para.AppendChild(new Run(new RunProperties(key.Value), new Text(key.Key)));
            }
        }

        return para;
    }

    public static ParagraphProperties GetParagraphProperties(HeadingLevel level) => new ParagraphProperties(new ParagraphStyleId() { Val = GetStyleId(level) });
    
    public static RunProperties GetRunProperties(HeadingLevel level) => new RunProperties(new RunStyle() { Val = GetStyleId(level) });

    private static string GetStyleId(HeadingLevel level)
    {
        return (level) switch
        {
            HeadingLevel.Title => "Titre",
            HeadingLevel.Subtitle => "Sous-titre",
            HeadingLevel.Normal => "Normal",
            HeadingLevel.Accent => "Accentuation",
            HeadingLevel.Contact => "Coordonnées",
            HeadingLevel.Icon => "Icônes",
            HeadingLevel.H1 => "Titre1",
            HeadingLevel.H2 => "Titre2",
            HeadingLevel.H3 => "Titre3",
            HeadingLevel.H4 => "Titre4",
            HeadingLevel.H5 => "Titre5",
            _ => "Normal"
        };
    }
    
    public static void SearchAndReplace(this WordprocessingDocument doc, string pTextToFind, string pTextReplacement)
    {
        var body = doc.MainDocumentPart!.Document.Body;
        var paras = body!.Elements<Paragraph>();

        foreach (var para in paras)
        {
            foreach (var run in para.Elements<Run>())
            {
                foreach (var text in run.Elements<Text>())
                {
                    if (text.Text.Contains(pTextToFind))
                    {
                        text.Text = text.Text.Replace(pTextToFind, pTextReplacement);
                    }
                }
            }
        }
    }
    
    public static T? Search<T>(this WordprocessingDocument doc, string pTextToFind) where T : OpenXmlElement
    {
        var body = doc.MainDocumentPart!.Document.Body;
        var paras = body!.Elements<Paragraph>();
        T? elem = default(T);
        
        foreach (var para in paras)
        {
            if (para.InnerText.Contains(pTextToFind, StringComparison.OrdinalIgnoreCase))
            {
                elem = para as T;
            }
        }

        return elem;
    }
    
    public static void SearchAndReplaceInTables(this WordprocessingDocument doc, string pTextToFind, string pTextReplacement)
    {
        var body = doc.MainDocumentPart!.Document.Body;
        var tables = body!.Elements<Table>();
        
        foreach (var table in tables)
        {
            var elements = table.Descendants<Paragraph>();
            foreach (var element in elements)
            {
                foreach (var run in element.Elements<Run>())
                {
                    foreach (var text in run.Elements<Text>())
                    {
                        if (text.Text.Contains(pTextToFind))
                        {
                            text.Text = text.Text.Replace(pTextToFind, pTextReplacement);
                        }
                    }
                }
            }
        }
    }
    
    
    /// <summary>
    /// Validator: many errors can be ignored due to the old version of the OpenXML validator. 
    /// This method is from the official OpenXML Microsoft's Documentation
    /// </summary>
    /// <param name="doc">The whole document after its edition right BEFORE his disposal !</param>
    public static void Validate(WordprocessingDocument doc)
    {
        try
        {
            var validator = new OpenXmlValidator();
            int count = 0;
            foreach (var error in validator.Validate(doc))
            {
                count++;
                Console.WriteLine("Error " + count);
                Console.WriteLine("Description: " + error.Description);
                Console.WriteLine("ErrorType: " + error.ErrorType);
                Console.WriteLine("Node: " + error?.Node);
                Console.WriteLine("Path: " + error?.Path?.XPath);
                Console.WriteLine("Part: " + error?.Part?.Uri);
                Console.WriteLine("-------------------------------------------");
            }

            Console.WriteLine("count={0}", count);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    public enum HeadingLevel
    {
        H1,
        H2,
        H3,
        H4,
        H5,
        Normal,
        Title,
        Subtitle,
        Icon,
        Contact,
        Accent
        
    }
}
