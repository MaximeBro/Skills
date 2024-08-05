using Skills.Models.Overview;

namespace Skills.Models.CV;

public class CvCertificationInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CvId { get; set; }
    public CvInfo? Cv { get; set; }
    public Guid CertificationId { get; set; }
    public UserCertificationInfo? Certification { get; set; }
}