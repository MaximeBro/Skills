using Microsoft.AspNetCore.Components;
using Skills.Models;

namespace Skills.Components.Pages.Overview;

public partial class SkillsProfile
{
    [Parameter] public UserModel User { get; set; } = null!;
}