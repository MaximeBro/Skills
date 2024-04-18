namespace Skills.Models.CV;

public class CvSafetyCertificationInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CvId { get; set; }
    public CvInfo? Cv { get; set; }
    public Guid CertId { get; set; }
    public SafetyCertification? Certification { get; set; }
}