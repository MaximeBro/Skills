namespace Skills.Models;

public class SkillModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
}