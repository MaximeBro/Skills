using System.Reflection;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Skills.Components.Components;

public partial class IconPicker : FullComponentBase
{
    [Parameter] public int LimitCount { get; set; }
    
    private Dictionary<string, string> _filled = [];
    private Dictionary<string, string> _outlined = [];
    private Dictionary<string, string> _rounded = [];
    private Dictionary<string, string> _sharp = [];
    private Dictionary<string, string> _twoTone = [];

    private string _search = string.Empty;
    
    private Func<KeyValuePair<string, string>, bool> QuickFilter => x =>
    {
        if (_filled.ContainsKey(_search)) return true;
        if (_outlined.ContainsKey(_search)) return true;
        if (_rounded.ContainsKey(_search)) return true;
        if (_sharp.ContainsKey(_search)) return true;
        if (_twoTone.ContainsKey(_search)) return true;
        if (string.IsNullOrWhiteSpace(_search)) return true;
        
        return false;
    };

    protected override void OnInitialized()
    {
        _filled = typeof(Icons.Material.Filled).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);
        _outlined = typeof(Icons.Material.Outlined).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);
        _rounded = typeof(Icons.Material.Rounded).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);
        _sharp = typeof(Icons.Material.Sharp).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);
        _twoTone = typeof(Icons.Material.TwoTone).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => x.GetValue(null)?.ToString() ?? string.Empty);

        if (LimitCount == 0)
        {
            LimitCount = Math.Max(Math.Max(Math.Max(_filled.Count, _outlined.Count), _rounded.Count), Math.Max(_sharp.Count, _twoTone.Count));
        }
    }

    private sealed class VirtualizableCollection<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public int Count { get; set; }
    }
}