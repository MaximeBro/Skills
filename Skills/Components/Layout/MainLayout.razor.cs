using Microsoft.AspNetCore.Components;
using MudBlazor;
using Skills.Services;

namespace Skills.Components.Layout;

public partial class MainLayout
{
    [Inject] public IWebHostEnvironment Environment { get; set; } = null!;
    [Inject] public ThemeManager ThemeManager { get; set; } = null!; 
    
    private MudTheme _theme = new();
    
    protected override void OnInitialized()
    {
        _theme.Palette.Primary = ThemeManager.GetMudColor(Color.Primary);
        _theme.Palette.Secondary = ThemeManager.GetMudColor(Color.Secondary);
    }
}