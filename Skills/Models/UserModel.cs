using Skills.Models.Enums;

namespace Skills.Models;

public class UserModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsDisabled { get; set; }
    public Guid? GroupId { get; set; }
    public GroupModel? Group { get; set; }
}