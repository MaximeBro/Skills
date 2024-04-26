using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Skills.Services;

namespace Skills.Components.Layout;

public partial class MainLayout
{
    [Inject] public IWebHostEnvironment Environment { get; set; } = null!;
    [Inject] public ThemeManager ThemeManager { get; set; } = null!; 
    
    private MudTheme _theme = new();
    
    protected override async Task OnInitializedAsync()
    {
        _theme.Palette.Primary = ThemeManager.GetMudColor(Color.Primary);
        _theme.Palette.Secondary = ThemeManager.GetMudColor(Color.Secondary);
    }
}