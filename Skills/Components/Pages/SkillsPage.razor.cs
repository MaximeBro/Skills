using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Models;

namespace Skills.Components.Pages;

public partial class SkillsPage : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    
    private Dictionary<Guid, List<TypeLevel>> _skillTypeLevels = new();
    private Dictionary<Guid, List<SoftTypeLevel>> _softSkillTypeLevels = new();
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
        var skillModels = await db.Skills.AsNoTracking()
            .Include(x => x.TypeInfo)
            .Include(x => x.CategoryInfo)
            .Include(x => x.SubCategoryInfo)
            .ToListAsync();

        var softSkillsModels = await db.SoftSkills.AsNoTracking()
            .Include(x => x.TypeInfo)
            .ToListAsync();

        foreach (var model in skillModels)
        {
            model.Type = model.TypeInfo.Value;
            model.Category = model.CategoryInfo.Value;
            model.SubCategory = model.SubCategoryInfo?.Value ?? string.Empty;
        }

        foreach (var model in softSkillsModels) model.Type = model.TypeInfo.Value;

        _models.Clear();
        _models.AddRange(new List<AbstractSkillModel>(skillModels));
        _models.AddRange(new List<AbstractSkillModel>(softSkillsModels));

        _skillTypeLevels.Clear();
        _softSkillTypeLevels.Clear();
        foreach (var model in _models) _skillTypeLevels.Add(model.Id, db.TypesLevels.AsNoTracking().Where(x => x.TypeId == model.TypeId).ToList());
        foreach (var model in _models) _softSkillTypeLevels.Add(model.Id, db.SoftTypesLevels.AsNoTracking().Where(x => x.SkillId == model.Id).ToList());
    }
}