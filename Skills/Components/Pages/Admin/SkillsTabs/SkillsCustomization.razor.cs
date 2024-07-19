using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Models.Enums;
using Skills.Services;

namespace Skills.Components.Pages.Admin.SkillsTabs;

public partial class SkillsCustomization : FullComponentBase
{
    [CascadingParameter(Name = "IsDarkMode")] public bool IsDarkMode { get; set; }
    [Parameter] public SkillsManagement Manager { get; set; } = null!;
    [Parameter] public string Title { get; set; } = string.Empty;
    
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IconHelperService IconHelperService { get; set; } = null!;
    [Inject] public ThemeManager ThemeManager { get; set; } = null!;
    
    private bool _loading;
    
    private List<SKillInfo> _types = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await RefreshPageDataAsync();
    }
    
    private async Task UpdateIconAsync(KeyValuePair<string, IconType> kvp, SKillInfo type)
    {
        var db = await Factory.CreateDbContextAsync();
        var old = db.SkillsTypes.FirstOrDefault(x => x.Id == type.Id);
        if (old != null)
        {
            old.Icon = kvp.Key;
            old.IconType = kvp.Value;
            await db.SaveChangesAsync();
        }
        await db.DisposeAsync();
        
        await RefreshPageDataAsync();
        StateHasChanged();
    }

    private async Task IconColorChangedAsync(Color color, SKillInfo type)
    {
        var db = await Factory.CreateDbContextAsync();
        var old = db.SkillsTypes.FirstOrDefault(x => x.Id == type.Id);
        if (old != null)
        {
            old.IconColor = color;
            await db.SaveChangesAsync();
        }
        await db.DisposeAsync();

        await RefreshPageDataAsync();
        StateHasChanged();
        
    }

    public async Task RefreshPageDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _types = db.SkillsTypes.AsNoTracking().Where(x => x.Type == SkillDataType.Type).ToList();
        await db.DisposeAsync();
    }
}