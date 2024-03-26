using Microsoft.AspNetCore.Components;
using Skills.Services;

namespace Skills.Components.Pages;

public partial class HomePage : ComponentBase
{
    [Inject] public ThemeManager ThemeManager { get; set; } = null!;
}