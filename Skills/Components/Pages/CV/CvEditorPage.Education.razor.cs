using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Models.CV;
using Skills.Models.Overview;

namespace Skills.Components.Pages.CV;

public partial class CvEditorPage_Education : FullComponentBase
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
    
    private void OnCheckChanged(bool value, UserEducationInfo edu)
    {
        Editor.CvEducations[edu] = value;
        _selectAll = Editor.CvEducations.All(x => x.Value);
        Editor.EditDone();
    }
    
    private void SelectAllChanged(bool value)
    {
        foreach (var key in Editor.CvEducations.Keys)
        {
            Editor.CvEducations[key] = value;
        }
        _selectAll = Editor.CvEducations.All(x => x.Value);
        Editor.EditDone();
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        var educations = await db.UserEducations.AsNoTracking().Where(x => x.UserId == Cv.UserId).ToListAsync();
        var associatedEducations = db.CvEducations.AsNoTracking().Include(x => x.Education)
            .Where(x => x.CvId == Cv.Id)
            .Select(x => x.EducationId).ToList();

        Editor.CvEducations.Clear();
        foreach (var education in educations)
        {
            Editor.CvEducations.Add(education, associatedEducations.Contains(education.Id));
        }
        
        await db.DisposeAsync();
        StateHasChanged();
    }
}