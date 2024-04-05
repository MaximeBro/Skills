using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Models;

namespace Skills.Components.Pages;

public partial class SkillsPage : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    
    private List<AbstractSkillModel> _models = new();
    private string _search = string.Empty;

    public Func<AbstractSkillModel, bool> QuickFilter => x =>
    {
        if (!string.IsNullOrWhiteSpace(x.Type) && x.Type.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (!string.IsNullOrWhiteSpace(x.Category) && x.Category.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (!string.IsNullOrWhiteSpace(x.SubCategory) && x.SubCategory.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
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
        var models = await db.Skills.AsNoTracking()
                                 .Include(x => x.TypeInfo)
                                 .Include(x => x.CategoryInfo)
                                 .Include(x => x.SubCategoryInfo)
                                 .ToListAsync();

        foreach (var model in models)
        {
            model.Type = model.TypeInfo.Value;
            model.Category = model.CategoryInfo.Value;
            model.SubCategory = model.SubCategoryInfo?.Value ?? string.Empty;
        }

        _models = new List<AbstractSkillModel>(models);
    }
}