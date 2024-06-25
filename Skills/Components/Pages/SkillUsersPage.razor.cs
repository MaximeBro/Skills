using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;

namespace Skills.Components.Pages;

public partial class SkillUsersPage : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Parameter] public Guid Id { get; set; }
    
    private AbstractSkillModel? _skill;
    private List<UserSkillModel> _users = new();
    private List<BreadcrumbItem> _breadcrumbs = new();
    private List<CommonTypeModel> _levels = new();

    protected override async Task OnInitializedAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _skill = db.Skills.FirstOrDefault(x => x.Id == Id) ?? db.SoftSkills.FirstOrDefault(x => x.Id == Id) as AbstractSkillModel;
        await db.DisposeAsync();

        if (_skill is null) NavManager.NavigateTo("/skills", true);
        _breadcrumbs.Add(new BreadcrumbItem("Accueil", "/"));
        _breadcrumbs.Add(new BreadcrumbItem("Compétences", "/skills"));
        _breadcrumbs.Add(new BreadcrumbItem($"{_skill!.ToStringNoType()}", null, true));
        
        await RefreshDataAsync();
    }

    public override async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _users = db.UsersSkills.AsNoTracking().Where(x => x.SkillId == Id).Include(x => x.User).ToList();
        _levels = _skill!.Type == "Soft-Skill" ? db.SoftTypesLevels.AsNoTracking().Where(x => x.SkillId == Id).Select(x => x.ToAbstract()).ToList() : 
                                                 db.TypesLevels.AsNoTracking().Where(x => x.TypeId == _skill.TypeId).Select(x => x.ToAbstract()).ToList();
        await db.DisposeAsync();
        StateHasChanged();
    }
}