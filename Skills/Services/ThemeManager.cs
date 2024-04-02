using MudBlazor.Utilities;
using Color = MudBlazor.Color;

namespace Skills.Services;

public class ThemeManager
{
    private readonly IConfiguration _configuration;
    public readonly string DefaultColor = "#fff";

    public ThemeManager(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Tries to get the specified files in in the config and copy them to the wwwroot app's folder. 
    /// </summary>
    /// <returns>A completed task.</returns>
    public Task InitAsync()
    {
        var files = new string[]
        {
            _configuration["favicon"] ?? "YourBrandIcon.ico",
            _configuration["drawer-icon"] ?? "YourBrandIcon.ico",
            _configuration["home-banner"] ?? "YourBrandBanner.png",
            _configuration["login-banner"] ?? string.Empty
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
    /// <returns>The HEX value of the specific color.</returns>
    public string GetColor(Color color) => _configuration[color.ToString().ToLower()] ?? DefaultColor;

    /// <summary>
    /// Retrieves the HEX value of the specified color which is set in the config and converts it to a <see cref="MudColor"/>.
    /// </summary>
    /// <param name="color">A <see cref="Color"/>.</param>
    /// <returns>A <see cref="MudColor"/> object that represents the value of the specific color.</returns>
    public MudColor GetMudColor(Color color) => new(GetColor(color));

    /// <summary>
    /// Retrieves the value of the specified key.
    /// </summary>
    /// <param name="key">The key as a string.</param>
    /// <returns>The value in set in the config or an empty string if not found.</returns>
    public string Get(string key) => _configuration[key] ?? string.Empty;
}