using Microsoft.AspNetCore.Components;
using MudBlazor;
using Skills.Services;

namespace Skills.Components.Layout;

public partial class MainLayout
{
    [Inject] public ThemeManager ThemeManager { get; set; } = null!; 
    private MudTheme _theme = new();

    protected override Task OnInitializedAsync()
    {
        _theme.Palette.Primary = ThemeManager.GetMudColor(Color.Primary);
        _theme.Palette.Secondary = ThemeManager.GetMudColor(Color.Secondary);

        return Task.CompletedTask;
    }
}