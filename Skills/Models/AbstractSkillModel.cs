using System.ComponentModel.DataAnnotations.Schema;
using Skills.Models.CV;

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
    
    public Guid? CvId { get; set; }
    public CvInfo? Cv { get; set; }
    
    public new string ToString() => !string.IsNullOrWhiteSpace(Type) ? (Type == "SOFT-SKILL" ? string.Join(" - ", Type, Description) : string.Join(" - ", Type, Category, SubCategory, Description)) : string.Empty;
    public new string ToStringNoType() => !string.IsNullOrWhiteSpace(Type) ? (Type == "SOFT-SKILL" ? string.Join(" - ", Type, Description) : string.Join(" - ", Category, SubCategory, Description)) : string.Empty;
    public new string ToStringNoTypeDesc() => !string.IsNullOrWhiteSpace(Type) ? (Type == "SOFT-SKILL" ? $"{Type}" : string.Join(" - ", Category, SubCategory)) : string.Empty;
    public new string ToStringNoDesc() => !string.IsNullOrWhiteSpace(Type) ? (Type == "SOFT-SKILL" ? $"{Type}" : string.Join(" - ", Type, Category, SubCategory)) : string.Empty;
}