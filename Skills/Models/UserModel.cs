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
    public string Username { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsDisabled { get; set; }
    public Guid? GroupId { get; set; }
    public GroupModel? Group { get; set; }
}