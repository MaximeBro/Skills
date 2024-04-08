using Microsoft.AspNetCore.Components;
using Skills.Components.Components;

namespace Skills.Components.Pages.Admin.SkillsTabs;

public partial class SoftSkills : FullComponentBase
{
    [Parameter] public SkillsManagement Manager { get; set; } = null!;
    [Parameter] public string Title { get; set; } = string.Empty;
}