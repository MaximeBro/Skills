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
}