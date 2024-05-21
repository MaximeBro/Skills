using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

namespace Skills.Models.CV;

public class CvInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    public DateTime BirthDate { get; set; }
    [MaxLength(length: 14)]
    public string PhoneNumber { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public int MinLevel { get; set; } = 2;

    public List<CvSkillInfo> Skills { get; set; } = new();
    public List<CvEducationInfo> Education { get; set; } = new();
    public List<CvExperienceInfo> Experiences { get; set; } = new();
    public List<CvCertificationInfo> Certifications { get; set; } = new();
    public List<CvSafetyCertificationInfo> SafetyCertifications { get; set; } = new();
}