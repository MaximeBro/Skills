using Microsoft.AspNetCore.Components;
using Skills.Services;

namespace Skills.Components.Components;

public partial class FullComponentBase : ComponentBase
{
    [Inject] public ThemeManager ThemeManager { get; set; } = null!;
}