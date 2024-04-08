namespace Skills.Models;

public class SoftTypeLevel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SkillId { get; set; }
    public SoftSkillModel? Skill { get; set; }

    public int Level { get; set; }
    public string Value { get; set; } = string.Empty;
}