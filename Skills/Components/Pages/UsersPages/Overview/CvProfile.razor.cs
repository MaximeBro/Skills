using Microsoft.AspNetCore.Components;
using Skills.Models;

namespace Skills.Components.Pages.UsersPages.Overview;

public partial class CvProfile : ComponentBase
{
    [Parameter] public UserModel User { get; set; } = null!;
}