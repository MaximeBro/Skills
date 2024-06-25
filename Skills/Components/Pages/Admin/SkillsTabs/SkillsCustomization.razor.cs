using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Models;
using Skills.Models.Enums;

namespace Skills.Components.Pages.Admin.SkillsTabs;

public partial class SkillsCustomization : FullComponentBase
{
    [Parameter] public SkillsManagement Manager { get; set; } = null!;
    [Parameter] public string Title { get; set; } = string.Empty;
    
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    
    private bool _loading;
    
    private List<SKillInfo> _types = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await RefreshPageDataAsync();
    }

    private async Task UpdateIconAsync(SKillInfo type, string? icon)
    {
        var db = await Factory.CreateDbContextAsync();
        var old = db.SkillsTypes.FirstOrDefault(x => x.Id == type.Id);
        if (old != null)
        {
            old.Icon = icon;
            await db.SaveChangesAsync();
        }
        await db.DisposeAsync();

        await Manager.RefreshSkillsAsync();
        await RefreshPageDataAsync();
        StateHasChanged();
    }

    public async Task RefreshPageDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _types = db.SkillsTypes.AsNoTracking().Where(x => x.Type == SkillDataType.Type && x.Value.ToUpper() != "SOFT-SKILL").ToList(); // It is very important to prevent anybody from editing the Soft-Skill Type, or it will break a lot of stuff !
        await db.DisposeAsync();
    }
}