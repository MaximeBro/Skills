using Skills.Extensions;
using Skills.Models.Enums;

namespace Skills.Models;

public abstract class AbstractSkillModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TypeId { get; set; }
    public SKillInfo TypeInfo { get; set; } = null!;
    
    public string? Type { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
    public string? Description { get; set; }
    
    public Guid? GroupId { get; set; }
    public GroupModel? Group { get; set; }
    
    public new string ToString() => !string.IsNullOrWhiteSpace(Type) ? (Type == Hardcoded.SoftSkill ? string.Join(" - ", Type, Description) : string.Join(" - ", Type, Category, SubCategory, Description)) : string.Empty;
    public string ToStringNoType() => !string.IsNullOrWhiteSpace(Type) ? (Type == Hardcoded.SoftSkill ? string.Join(" - ", Type, Description) : string.Join(" - ", Category, SubCategory, Description)) : string.Empty;
    public string ToStringNoTypeNoDesc() => !string.IsNullOrWhiteSpace(Type) ? (Type == Hardcoded.SoftSkill ? $"{Type}" : string.Join(" - ", Category, SubCategory)) : string.Empty;
    public string ToStringNoDesc() => !string.IsNullOrWhiteSpace(Type) ? (Type == Hardcoded.SoftSkill ? $"{Type}" : string.Join(" - ", Type, Category, SubCategory)) : string.Empty;
}