using System.ComponentModel;

namespace Skills.Models.Enums;

public enum UserRole
{
    [Description("Invité")]
    Anonymous,
    
    [Description("Utilisateur")]
    User,
    
    [Description("Manager")]
    Manager,
    
    [Description("Administrateur")]
    Admin
}