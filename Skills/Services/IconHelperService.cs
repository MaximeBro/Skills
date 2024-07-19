using System.Reflection;
using MudBlazor;
using Skills.Models.Enums;

namespace Skills.Services;

public class IconHelperService
{
    public Dictionary<string, string> Filled { get; }
    public Dictionary<string, string> Outlined { get; }
    public Dictionary<string, string> Rounded { get; }
    public Dictionary<string, string> Sharp { get; }
    public Dictionary<string, string> TwoTone { get; }

    public IconHelperService()
    {
        Filled = typeof(Icons.Material.Filled).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);
        Outlined = typeof(Icons.Material.Outlined).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);
        Rounded = typeof(Icons.Material.Rounded).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);
        Sharp = typeof(Icons.Material.Sharp).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);
        TwoTone = typeof(Icons.Material.TwoTone).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);
    }

    public string Get(string? name, IconType type)
    {
        if (string.IsNullOrWhiteSpace(name)) return Filled.FirstOrDefault(x => x.Key.Contains("ContactSupport")).Value;
        
        return type switch
        {
            IconType.Filled => Filled.FirstOrDefault(x => x.Key == name).Value,
            IconType.Outlined => Outlined.FirstOrDefault(x => x.Key == name).Value,
            IconType.Rounded => Rounded.FirstOrDefault(x => x.Key == name).Value,
            IconType.Sharp => Sharp.FirstOrDefault(x => x.Key == name).Value,
            IconType.TwoTone => TwoTone.FirstOrDefault(x => x.Key == name).Value,
            _ => Filled.FirstOrDefault(x => x.Key.Contains("ContactSupport")).Value
        };
    }
}