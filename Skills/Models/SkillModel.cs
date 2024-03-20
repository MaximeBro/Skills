namespace Skills.Models;

public class SkillModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<UserModel> Users { get; set; } = new();
    public List<UserSkillModel> UserSkills { get; set; } = new();
    public string Type { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public string Description { get; set; }
}