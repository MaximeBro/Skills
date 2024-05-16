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
    
    private MudTheme _theme = new();
    private bool _isDarkMode;
    
    protected override void OnInitialized()
    {
        _theme.Palette = ThemeManager.GetLightPalette();
        _theme.PaletteDark = ThemeManager.GetDarkPalette();
        
        ThemeManager.OnPaletteChanged += RefreshPalettes;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _isDarkMode = ThemeManager.IsDarkTheme = await LocalStorage.GetItemAsync<bool>("IsDarkTheme");
            var language = await LocalStorage.GetItemAsStringAsync("PreferredLanguage");
            if (!string.IsNullOrWhiteSpace(language))
            {
                await Lang.SetLanguageAsync(language);
            }
            
            StateHasChanged();
        }
    }

    private void RefreshPalettes()
    {
        _theme.Palette = ThemeManager.GetLightPalette();
        _theme.PaletteDark = ThemeManager.GetDarkPalette();
        _isDarkMode = ThemeManager.IsDarkTheme;
    }
}