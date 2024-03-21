using Skills.Models.Enums;

namespace Skills.Models;

public class SKillInfo
{
    public Guid Id { get; set; }
    public string Value { get; set; }
    public SkillInfoType InfoType { get; set; }
}