namespace Skills.Models.CV;

public class CvExperienceInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CvId { get; set; }
    public CvInfo? Cv { get; set; }
    public string Title { get; set; } = string.Empty;
    public string DateInfo { get; set; } = string.Empty;
}