using Skills.Models.CV;

namespace Skills.Models.Overview;

public class UserCertificationInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public UserModel? User { get; set; }
    public List<CvInfo> CVs { get; set; } = [];
    
    public string Title { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Duration { get; set; } = string.Empty;
}