using MudBlazor.Utilities;
using Color = MudBlazor.Color;

namespace Skills.Services;

public class ThemeService
{
    private readonly IConfiguration _configuration;
    public readonly string DefaultColor = "#fff";

    public ThemeService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string GetColor(Color color) => _configuration[color.ToString()] ?? DefaultColor;

    public MudColor GetMudColor(Color color) => new(GetColor(color));
}