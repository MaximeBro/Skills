using System.ComponentModel.DataAnnotations.Schema;
using Skills.Models.Enums;

namespace Skills.Models;

public class UserModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<SkillModel> Skills { get; set; } = new();
    public List<UserSkillModel> UserSkills { get; set; } = new();
    public string Name { get; set; }
    public string Email { get; set; }
    public PermissionPolicy Policy { get; set; }
    public bool IsDisabled { get; set; }

    [NotMapped]
    public string Username => Email.Replace("@sasp.fr", string.Empty);
}