using Skills.Models.CV;

namespace Skills.Models.Overview;

public class UserExperienceInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public UserModel? User { get; set; }
    public List<CvInfo> CVs { get; set; } = [];

    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string Description { get; set; } = string.Empty;
}