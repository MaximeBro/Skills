using MudBlazor;
using MudBlazor.Utilities;
using Color = MudBlazor.Color;

namespace Skills.Services;

public class ThemeManager(IConfiguration configuration)
{
    public readonly string DefaultColor = "#fff";
    
    /// <summary>
    /// Tries to get the specified files in the config and copy them to the wwwroot app's folder. 
    /// </summary>
    /// <returns>A completed task.</returns>
    public Task InitAsync()
    {
        var files = new[]
        {
            configuration["favicon"] ?? "YourBrandIcon.ico",
            configuration["drawer-icon"] ?? "YourBrandIcon.ico",
            configuration["home-banner"] ?? "YourBrandBanner.png",
            configuration["login-banner"] ?? string.Empty
        };

        foreach (var file in files)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\data\\config\\", file);
            var resourcePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\pictures\\", file);
            if (File.Exists(filePath))
            {
                if (File.Exists(resourcePath)) File.Delete(resourcePath);
                File.Copy(filePath, resourcePath);
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves the HEX value of the specified color which is set in the config.
    /// </summary>
    /// <param name="color">A <see cref="Color"/>.</param>
    /// <param name="isDarkTheme">If the color is related to the dark or the light theme (optional)</param>
    /// <returns>The HEX value of the specific color.</returns>
    public string GetColor(Color color, bool isDarkTheme = false)
    {
        var theme = isDarkTheme ? "DarkTheme" : "LightTheme";
        var colors = configuration.GetSection(theme).GetChildren().ToList();
        return colors[(int)color-1].GetSection(color.ToString()).Value ?? DefaultColor;
    }

    /// <summary>
    /// Retrieves the HEX value of the specified color which is set in the config and converts it to a <see cref="MudColor"/>.
    /// </summary>
    /// <param name="color">A <see cref="Color"/>.</param>
    /// /// <param name="isDarkTheme">If the color is related to the dark or the light theme (optional)</param>
    /// <returns>An <see cref="MudColor"/> object that represents the value of the specific color.</returns>
    public MudColor GetMudColor(Color color, bool isDarkTheme = false) => new(GetColor(color, isDarkTheme));

    /// <summary>
    /// Retrieves the value of the specified key.
    /// </summary>
    /// <param name="key">The key as a string.</param>
    /// <returns>The value in set in the config or an empty string if not found.</returns>
    public string Get(string key) => configuration[key] ?? string.Empty;

    public PaletteLight GetLightPalette()
    {
        return new PaletteLight
        {
            Primary = GetMudColor(Color.Primary),
            Secondary = GetMudColor(Color.Secondary),
            Tertiary = GetMudColor(Color.Tertiary),
            Info = GetMudColor(Color.Info),
            Success = GetMudColor(Color.Success),
            Warning = GetMudColor(Color.Warning),
            Error = GetMudColor(Color.Error),
        };
    }
    
    public PaletteDark GetDarkPalette()
    {
        return new PaletteDark
        {
            Primary = GetMudColor(Color.Primary, true),
            Secondary = GetMudColor(Color.Secondary, true),
            Tertiary = GetMudColor(Color.Tertiary, true),
            Info = GetMudColor(Color.Info, true),
            Success = GetMudColor(Color.Success, true),
            Warning = GetMudColor(Color.Warning, true),
            Error = GetMudColor(Color.Error, true),
        };
    }
}