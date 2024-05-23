using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using MudBlazor;
using Skills.Components.Components;
using Skills.Components.Dialogs.CV;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models.CV;
using Skills.Models.Overview;

namespace Skills.Components.Pages.CV;

public partial class CvEditorPage_Experiences : FullComponentBase
{
    [CascadingParameter(Name = "cv-editor")] public CvEditorPage Editor { get; set; } = null!;
    [Parameter] public CvInfo Cv { get; set; } = null!;
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;

    private bool _selectAll;
    
    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
        StateHasChanged();
    }
    
    private void OnCheckChanged(bool value, UserExperienceInfo exp)
    {
        Editor.CvExperiences[exp] = value;
        _selectAll = Editor.CvExperiences.All(x => x.Value);
        Editor.EditDone();
    }
    
    private void SelectAllChanged(bool value)
    {
        foreach (var key in Editor.CvExperiences.Keys)
        {
            Editor.CvExperiences[key] = value;
        }
        _selectAll = Editor.CvExperiences.All(x => x.Value);
        Editor.EditDone();
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        var experiences = await db.UserExperiences.AsNoTracking().Where(x => x.UserId == Cv.UserId).ToListAsync();
        var associatedExperiences = db.CvExperiences.AsNoTracking().Include(x => x.Experience)
            .Where(x => x.CvId == Cv.Id)
            .Select(x => x.ExperienceId).ToList();
        
        Editor.CvExperiences.Clear();
        foreach (var experience in experiences)
        {
            Editor.CvExperiences.Add(experience, associatedExperiences.Contains(experience.Id));
        }
        await db.DisposeAsync();
    }
}