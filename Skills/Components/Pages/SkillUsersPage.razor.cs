using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Models;

namespace Skills.Components.Pages;

public partial class SkillUsersPage : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Parameter] public Guid Id { get; set; }
    
    private AbstractSkillModel? _skill;
    private bool _isSoftSkill = false;
    private List<UserSkillModel> _users = new();
    private List<BreadcrumbItem> _breadcrumbs = new();

    protected override async Task OnInitializedAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _skill = db.Skills.FirstOrDefault(x => x.Id == Id);
        if (_skill is null)
        {
            _skill = db.SoftSkills.FirstOrDefault(x => x.Id == Id);
            _isSoftSkill = true;
        }
        await db.DisposeAsync();

        if (_skill is null) NavManager.NavigateTo("/skills", true);
        _breadcrumbs.Add(new BreadcrumbItem("Accueil", "/"));
        _breadcrumbs.Add(new BreadcrumbItem("Compétences", "/skills"));
        _breadcrumbs.Add(new BreadcrumbItem($"{_skill!.ToString()}", null, true));
        
        await RefreshDataAsync();
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _users = db.Userskills.AsNoTracking().Where(x => x.SkillId == Id).Include(x => x.User).ToList();
        await db.DisposeAsync();
        StateHasChanged();
    }
}