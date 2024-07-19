using Microsoft.AspNetCore.Components;
using MudBlazor;
using Skills.Models.Enums;
using Skills.Services;

namespace Skills.Components.Components;

public partial class IconPicker : FullComponentBase
{
    [Inject] public IconHelperService IconHelperService { get; set; } = null!;
    
    [Parameter] public int LimitCount { get; set; }
    
    [Parameter] public EventCallback<KeyValuePair<string, IconType>> OnClick { get; set; }
    
    private Dictionary<string, string> _filled = [];
    private Dictionary<string, string> _outlined = [];
    private Dictionary<string, string> _rounded = [];
    private Dictionary<string, string> _sharp = [];
    private Dictionary<string, string> _twoTone = [];

    private string _search = string.Empty;
    private MudMenu _menu = null!;
    
    private Func<KeyValuePair<string, string>, bool> QuickFilter => x =>
    {
        if (x.Key.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (string.IsNullOrWhiteSpace(_search)) return true;
        
        return false;
    };

    protected override void OnInitialized()
    {
        _filled = IconHelperService.Filled;
        _outlined = IconHelperService.Outlined;
        _rounded = IconHelperService.Rounded;
        _sharp = IconHelperService.Sharp;
        _twoTone = IconHelperService.TwoTone;
        
        if (LimitCount == 0)
        {
            LimitCount = Math.Max(Math.Max(Math.Max(_filled.Count, _outlined.Count), _rounded.Count), Math.Max(_sharp.Count, _twoTone.Count));
        }
    }

    private async Task InvokeItemChangedAsync(string item, IconType type)
    {
        await OnClick.InvokeAsync(new KeyValuePair<string, IconType>(item, type));
        _menu.CloseMenu();
        await InvokeAsync(StateHasChanged);
    }
}