namespace Skills.Models.Enums;

[Flags]
public enum PermissionPolicy
{
    Read = 1,
    Write = 2,
    Edit = 4,
    Delete = 8,
    Users = 16,
    Skills = 32,
    Root = 64
}