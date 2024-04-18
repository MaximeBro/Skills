namespace Skills.Models.CV;

public class CvSkillInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CvId { get; set; }
    public CvInfo? Cv { get; set; }
    public Guid SkillId { get; set; }
    public AbstractSkillModel? Skill { get; set; }
    public bool IsSoftSkill { get; set; }
}