using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Models;

namespace Skills.Components.Pages;

public partial class SkillsPage : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    
    private List<SkillModel> _models = new();
    private string _search = string.Empty;

    public Func<SkillModel, bool> QuickFilter => x =>
    {
        if (x.Type.Value.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Category.Value.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.SubCategory != null && x.SubCategory.Value.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (!string.IsNullOrWhiteSpace(x.Description) && x.Description.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;

        return false;
    };

    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _models = await db.Skills.AsNoTracking()
                                 .Include(x => x.Type)
                                 .Include(x => x.Category)
                                 .Include(x => x.SubCategory)
                                 .ToListAsync();
    }
}