using Skills.Models.Enums;

namespace Skills.Models;

public class SKillInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Value { get; set; } = string.Empty;
    public SkillDataType Type { get; set; }
}