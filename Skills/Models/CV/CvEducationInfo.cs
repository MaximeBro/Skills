using Skills.Models.Overview;

namespace Skills.Models.CV;

public class CvEducationInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CvId { get; set; }
    public CvInfo? Cv { get; set; }
    public Guid EducationId { get; set; }
    public UserEducationInfo? Education { get; set; }
}