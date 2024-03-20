using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Skills.Databases;
using Skills.Models;

namespace Skills.Components.Pages;

public partial class SkillsPage
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;

    private List<SkillModel> _skills = new();

    private string _search = string.Empty;
    public Func<SkillModel, bool> QuickFilter => x =>
    {
        if (x.Type.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Category.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.SubCategory.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Description.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;

        return false;
    };
    
    protected override async Task OnInitializedAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _skills = await db.Skills.AsNoTracking().ToListAsync();
    }
}