using MudBlazor;
using Skills.Models.Enums;

namespace Skills.Models;

public class SKillInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Value { get; set; } = string.Empty;
    public SkillDataType Type { get; set; }
    
    public string? Icon { get; set; }
    public IconType IconType { get; set; } = IconType.Unknown;
    public Color IconColor { get; set; } = Color.Primary;
}