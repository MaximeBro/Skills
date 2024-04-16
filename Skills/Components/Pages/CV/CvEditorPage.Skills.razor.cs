using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Models;
using Skills.Models.CV;

namespace Skills.Components.Pages.CV;

public partial class CvEditorPage_Skills : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Parameter] public CvInfo Cv { get; set; } = null!;

    private int _level = 3;
    
    public Dictionary<AbstractSkillModel, bool> ChosenSkills = new();
    private Dictionary<string, List<AbstractSkillModel>>  _skills = new();
    
    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
    }

    private async Task LevelChangedAsync(int level)
    {
        _level = level;
        await RefreshDataAsync();
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        var skills = db.UsersSkills.AsNoTracking().Where(x => x.Level >= _level && x.UserId == Cv.UserId)
                                                                   .Include(x => x.Skill)
                                                                   .Select(x => x.Skill).ToList() as List<AbstractSkillModel>;
        
        ChosenSkills.Clear();
        foreach (var skill in skills)
        {
            var selected = Cv.Skills.Select(x => x.Id).Contains(skill.Id) ||
                           Cv.SoftSkills.Select(x => x.Id).Contains(skill.Id);
            ChosenSkills.Add(skill, selected);
        }

        _skills = skills.GroupBy(x => x.Type ?? string.Empty).ToDictionary(x => x.Key, y => y.ToList());
        await db.DisposeAsync();
        StateHasChanged();
    }
}