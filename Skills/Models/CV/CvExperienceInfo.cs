using Skills.Models.Overview;

namespace Skills.Models.CV;

public class CvExperienceInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CvId { get; set; }
    public CvInfo? Cv { get; set; }
    public Guid ExperienceId { get; set; }
    public UserExperienceInfo? Experience { get; set; }
}