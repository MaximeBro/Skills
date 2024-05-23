using Skills.Models.Overview;

namespace Skills.Models.CV;

public class CvCertificationInfo
{
    public Guid CertificationId { get; set; } = Guid.NewGuid();
    public UserCertificationInfo? Certification { get; set; }
    public Guid CvId { get; set; }
    public CvInfo? Cv { get; set; }
}