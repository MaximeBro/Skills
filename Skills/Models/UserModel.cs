using Skills.Models.Enums;

namespace Skills.Models;

public class UserModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; }
    public string Email { get; set; }
    public PermissionPolicy Policy { get; set; }
    public bool IsDisabled { get; set; }
}