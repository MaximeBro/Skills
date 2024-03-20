using Microsoft.AspNetCore.Components;
using Skills.Models;

namespace Skills.Components.Pages.Overview;

public partial class CvProfile
{
    [Parameter] public UserModel User { get; set; } = null!;
}