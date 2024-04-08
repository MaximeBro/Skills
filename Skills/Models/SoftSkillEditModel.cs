namespace Skills.Models;

public class SoftSkillEditModel
{
    public AbstractSkillModel SoftSkill { get; set; } = null!;
    public List<SoftTypeLevel> Levels { get; set; } = new();
}