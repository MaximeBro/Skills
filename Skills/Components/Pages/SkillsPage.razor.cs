using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Models;
using Skills.Services;

namespace Skills.Components.Pages;

public partial class SkillsPage : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IconHelperService IconHelperService { get; set; } = null!;

    private List<BreadcrumbItem> _breadcrumbs = new();
    private Dictionary<Guid, List<TypeLevel>> _skillTypeLevels = new();
    private Dictionary<Guid, List<SoftTypeLevel>> _softSkillTypeLevels = new();
    private List<AbstractSkillModel> _models = new();

    private string _search = string.Empty;

    private bool _loading;

    private MudDataGrid<AbstractSkillModel> _grid = null!;

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
        _breadcrumbs.Add(new BreadcrumbItem("Accueil", "/"));
        _breadcrumbs.Add(new BreadcrumbItem("Compétences", null, true));
        
        await RefreshDataAsync();
        StateHasChanged();
    }

    private void OnRowClicked(DataGridRowClickEventArgs<AbstractSkillModel> args)
    {
        if (args.MouseEventArgs.Detail == 2)
        {
            NavManager.NavigateTo($"/skill-users/{args.Item.Id}");
        }
    }

    public override async Task RefreshDataAsync()
    {
        _loading = true;
        var db = await Factory.CreateDbContextAsync();
        var skillModels = await db.Skills.AsNoTracking().Include(x => x.TypeInfo).ToListAsync();
        var softSkillsModels = await db.SoftSkills.AsNoTracking().Include(x => x.TypeInfo).ToListAsync();

        _models.Clear();
        _models.AddRange(new List<AbstractSkillModel>(skillModels));
        _models.AddRange(new List<AbstractSkillModel>(softSkillsModels));
       
        _models = _models.Where(QuickFilter).ToList();
        
        _skillTypeLevels.Clear();
        _softSkillTypeLevels.Clear();
        foreach (var model in _models) _skillTypeLevels.Add(model.Id, db.TypesLevels.AsNoTracking().Where(x => x.TypeId == model.TypeId).ToList());
        foreach (var model in _models) _softSkillTypeLevels.Add(model.Id, db.SoftTypesLevels.AsNoTracking().Where(x => x.SkillId == model.Id).ToList());
        
        _loading = false;
    }
}