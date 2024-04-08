namespace Skills.Models;

public class SkillModel : AbstractSkillModel
{
    public Guid CategoryId { get; set; }
    public SKillInfo CategoryInfo { get; set; } = null!;
    
    public Guid? SubCategoryId { get; set; }
    public SKillInfo? SubCategoryInfo { get; set; }
}