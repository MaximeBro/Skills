namespace Skills.Models;

public class UserSkillModel
{
    public Guid UserId { get; set; }
    public Guid SkillId { get; set; }
    public int Level { get; set; }
}