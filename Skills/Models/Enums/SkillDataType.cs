using System.ComponentModel;

namespace Skills.Models.Enums;

public enum SkillDataType
{
    [Description("Type")]
    Type,
    
    [Description("Catégorie")]
    Category,
    
    [Description("Sous catégorie")]
    SubCategory
}