namespace Skills.Models.CV;

public class CvEducationInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CvId { get; set; }
    public CvInfo? Cv { get; set; }
    public int YearStart { get; set; }
    public int YearEnd { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}