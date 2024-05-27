using Skills.Models.CV;

namespace Skills.Models.Overview;

public class UserEducationInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public UserModel? User { get; set; }
    public List<CvInfo> CVs { get; set; } = [];
    public int YearStart { get; set; }
    public int YearEnd { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}