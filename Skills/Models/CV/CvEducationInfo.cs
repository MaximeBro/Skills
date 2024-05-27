using Skills.Models.Overview;

namespace Skills.Models.CV;

public class CvEducationInfo
{
    public Guid EducationId { get; set; } = Guid.NewGuid();
    public UserEducationInfo? Education { get; set; }
    public Guid CvId { get; set; }
    public CvInfo? Cv { get; set; }
}