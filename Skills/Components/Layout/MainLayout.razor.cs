using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Skills.Services;

namespace Skills.Components.Layout;

public partial class MainLayout
{
    [Inject] public IWebHostEnvironment Environment { get; set; } = null!;
    [Inject] public ILocalStorageService LocalStorage { get; set; } = null!;
    [Inject] public ThemeManager ThemeManager { get; set; } = null!;
    [Inject] public LocalizationManager Lang { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!; 
    
    private readonly MudTheme _theme = new();
    public bool IsDarkMode { get; private set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _theme.Palette = ThemeManager.GetLightPalette();
            _theme.PaletteDark = ThemeManager.GetDarkPalette();
            
            IsDarkMode = await LocalStorage.GetItemAsync<bool>("IsDarkTheme");
            var language = await LocalStorage.GetItemAsStringAsync("PreferredLanguage");
            if (!string.IsNullOrWhiteSpace(language))
            {
                await Lang.SetLanguageAsync(language);
            }
            
            StateHasChanged();
        }
    }

    public async Task RefreshPalettesAsync(bool isDarkMode)
    {
        _theme.Palette = ThemeManager.GetLightPalette();
        _theme.PaletteDark = ThemeManager.GetDarkPalette();
        IsDarkMode = isDarkMode;
        await InvokeAsync(StateHasChanged);
    }
}