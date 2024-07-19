namespace Skills.Models;

public class UserSkillModel
{
    public Guid UserId { get; set; }
    public UserModel? User { get; set; }
    public Guid SkillId { get; set; }
    public AbstractSkillModel? Skill { get; set; }
    public int Level { get; set; }
}