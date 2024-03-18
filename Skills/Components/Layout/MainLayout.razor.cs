using Microsoft.AspNetCore.Components;
using MudBlazor;
using Skills.Services;

namespace Skills.Components.Layout;

public partial class MainLayout
{
    [Inject] public ThemeService ThemeService { get; set; } = null!; 
    private MudTheme _theme = new();

    protected override Task OnInitializedAsync()
    {
        _theme.Palette.Primary = ThemeService.GetMudColor(Color.Primary);
        _theme.Palette.Secondary = ThemeService.GetMudColor(Color.Secondary);

        return Task.CompletedTask;
    }
}