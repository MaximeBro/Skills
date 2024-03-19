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
    
    public string GetColor(Color color) => _configuration[color.ToString().ToLower()] ?? DefaultColor;

    public MudColor GetMudColor(Color color) => new(GetColor(color));

    public string Get(string key) => _configuration[key] ?? string.Empty;
}