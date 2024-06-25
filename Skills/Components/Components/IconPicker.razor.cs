using System.Reflection;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Skills.Components.Components;

public partial class IconPicker : ComponentBase
{
    private Dictionary<string, string> _filled = [];
    private Dictionary<string, string> _outlined = [];
    private Dictionary<string, string> _rounded = [];
    private Dictionary<string, string> _sharp = [];
    private Dictionary<string, string> _twoTone = [];
    
    public bool IsVisible { get; set; }

    protected override void OnInitialized()
    {
        _filled = typeof(Icons.Material.Filled).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);
        _outlined = typeof(Icons.Material.Outlined).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);
        _rounded = typeof(Icons.Material.Rounded).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);
        _sharp = typeof(Icons.Material.Sharp).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);
        _twoTone = typeof(Icons.Material.TwoTone).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);
    }
    
    
}