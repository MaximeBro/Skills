namespace Skills.Models;

public class SkillModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<UserModel> Users { get; set; } = new();
    public List<UserSkillModel> UserSkills { get; set; } = new();
    
    public Guid TypeId { get; set; }
    public SKillInfo Type { get; set; }
    
    public Guid CategoryId { get; set; }
    public SKillInfo Category { get; set; }
    
    public Guid? SubCategoryId { get; set; }
    public SKillInfo? SubCategory { get; set; }

    public string? Description { get; set; }
    
    public Guid? GroupId { get; set; }
    public GroupModel? Group { get; set; }
}