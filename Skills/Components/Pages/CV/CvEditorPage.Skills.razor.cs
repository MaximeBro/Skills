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
    
    [CascadingParameter(Name = "cv-editor")] public CvEditorPage Editor { get; set; } = null!;
    [Parameter] public CvInfo Cv { get; set; } = null!;

    private bool _selectAll;
    
    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
        StateHasChanged();
    }

    private async Task LevelChangedAsync(int level)
    {
        Cv.MinLevel = level;
        await RefreshDataAsync();
        _selectAll = false;
        StateHasChanged();
    }

    private void OnCheckChanged(bool value, AbstractSkillModel skill)
    {
        Editor.ChosenSkills[skill] = value;
        _selectAll = Editor.ChosenSkills.All(x => x.Value);
        Editor.EditDone();
    }

    private void SelectAllChanged(bool value)
    {
        foreach (var key in Editor.ChosenSkills.Keys)
        {
            Editor.ChosenSkills[key] = value;
        }
        _selectAll = Editor.ChosenSkills.All(x => x.Value);
        Editor.EditDone();
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        var cvSkills = db.CvSkills.AsNoTracking().Where(x => x.CvId == Cv.Id).Select(x => x.SkillId).ToList();
        var skills = db.UsersSkills.AsNoTracking().Where(x => x.Level >= Cv.MinLevel && x.UserId == Cv.UserId)
                                                                   .Include(x => x.Skill)
                                                                   .Select(x => x.Skill).ToList() as List<AbstractSkillModel>;

        var t = db.UsersSkills.AsNoTracking().Where(x => x.Level >= Cv.MinLevel && x.UserId == Cv.UserId).ToList();
        
        Editor.ChosenSkills.Clear();
        foreach (var skill in skills)
        {
            Editor.ChosenSkills.Add(skill, cvSkills.Contains(skill.Id));
        }
        await db.DisposeAsync();
        
        _selectAll = Editor.ChosenSkills.All(x => x.Value);
    }
}