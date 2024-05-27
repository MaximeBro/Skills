using Skills.Models.Overview;

namespace Skills.Models.CV;

public class CvExperienceInfo
{
    public Guid ExperienceId { get; set; } = Guid.NewGuid();
    public UserExperienceInfo? Experience { get; set; }
    public Guid CvId { get; set; }
    public CvInfo? Cv { get; set; }
}