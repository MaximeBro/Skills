namespace Skills.Models.CV;

public class CvCertificationInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CvId { get; set; }
    public CvInfo? Cv { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Duration { get; set; } = string.Empty;
}