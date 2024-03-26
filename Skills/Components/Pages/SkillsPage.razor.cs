using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Skills.Databases;
using Skills.Models;

namespace Skills.Components.Pages;

public partial class SkillsPage : ComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    
    private List<SkillModel> _models = new();
    private string _search = string.Empty;

    public Func<SkillModel, bool> QuickFilter => x =>
    {
        if (x.Type != null && x.Type.Value.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Category != null && x.Category.Value.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.SubCategory != null && x.SubCategory.Value.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Description.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;

        return false;
    };

    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _models = await db.Skills.AsNoTracking().ToListAsync();
    }
}