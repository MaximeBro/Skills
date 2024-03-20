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

    public Task InitAsync()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\data\\config\\", _configuration["favicon"] ?? "YourBrandIcon.png");
        var resourcePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\pictures\\", _configuration["favicon"] ?? "YourBrandIcon.png");
        if (File.Exists(filePath))
        {
            if (File.Exists(resourcePath)) File.Delete(resourcePath);
            File.Copy(filePath, resourcePath);
        }
        
        return Task.CompletedTask;
    }
    
    public string GetColor(Color color) => _configuration[color.ToString().ToLower()] ?? DefaultColor;

    public MudColor GetMudColor(Color color) => new(GetColor(color));

    public string Get(string key) => _configuration[key] ?? string.Empty;
}