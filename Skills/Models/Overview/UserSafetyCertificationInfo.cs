using Skills.Models.CV;

namespace Skills.Models.Overview;

public class UserSafetyCertificationInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public UserModel? User { get; set; }
    public List<CvInfo> CVs { get; set; } = [];
    public Guid CertId { get; set; }
    public SafetyCertification? Certification { get; set; }
}