using System.ComponentModel.DataAnnotations;
using Skills.Models.Overview;

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

    public List<CvSkillInfo> Skills { get; set; } = [];
    public List<UserEducationInfo> Educations { get; set; } = [];
    public List<UserExperienceInfo> Experiences { get; set; } = [];
    public List<UserCertificationInfo> Certifications { get; set; } = [];
    public List<CvSafetyCertificationInfo> SafetyCertifications { get; set; } = [];
}