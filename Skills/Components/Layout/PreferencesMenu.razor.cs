using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Skills.Components.Components;

namespace Skills.Components.Layout;

public partial class PreferencesMenu : FullComponentBase
{
    [Inject] public ILocalStorageService LocalStorage { get; set; } = null!;
    [CascadingParameter(Name = "MainLayout")] public MainLayout Layout { get; set; } = null!;
    [Parameter] public string Class { get; set; } = null!;

    private MudMenu _menu = null!;

    private bool _isDarkMode;
    private bool _french = true;

    private string _frenchSvg = @"<svg xmlns=""http://www.w3.org/2000/svg"" height=""24"" viewBox=""0 -960 960 960"" width=""24""><path d=""M160-280v-400h280v80H240v80h160v80H240v160h-80Zm360 0v-400h200q33 0 56.5 23.5T800-600v80q0 32-22 54.5T726-440l74 160h-84l-75-160h-41v160h-80Zm80-240h120v-80H600v80Z""/></svg>";
    private string _englishSvg = @"<svg xmlns=""http://www.w3.org/2000/svg"" height=""24"" viewBox=""0 -960 960 960"" width=""24""><path d=""M240-280q-33 0-56.5-23.5T160-360v-320h80v320h120v-320h80v320q0 33-23.5 56.5T360-280H240Zm360 0q-33 0-56.5-23.5T520-360v-40h80v40h120v-80H600q-33 0-56.5-23.5T520-520v-80q0-33 23.5-56.5T600-680h120q33 0 56.5 23.5T800-600v40h-80v-40H600v80h120q33 0 56.5 23.5T800-440v80q0 33-23.5 56.5T720-280H600Z""/></svg>";


    protected override void OnInitialized()
    {
        _isDarkMode = ThemeManager.IsDarkTheme;
        _french = Lang.SelectedLanguage == "fr-FR";
        StateHasChanged();
    }

    private async Task SwitchThemeAsync(bool value)
    {
        _isDarkMode = value;
        ThemeManager.SetPalette(value);
        await LocalStorage.SetItemAsync("IsDarkTheme", value);
        StateHasChanged();
    }

    private async Task SwitchLanguageAsync(bool value)
    {
        _french = value;
        var language = _french ? "fr-FR" : "en-US";
        await Lang.SetLanguageAsync(language);
        await LocalStorage.SetItemAsync("PreferredLanguage", language);
        StateHasChanged();
    }
}