using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Components.Dialogs.CV;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models.CV;

namespace Skills.Components.Pages.CV;

public partial class CvEditorPage_Experiences : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    
    [CascadingParameter(Name = "cv-editor")] public CvEditorPage Editor { get; set; } = null!;
    [Parameter] public CvInfo Cv { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
    }

    private async Task CreateExperienceAsync()
    {
        var options = Hardcoded.DialogOptions;
        options.MaxWidth = MaxWidth.Medium;
        var instance = await DialogService.ShowAsync<ExperienceDialog>(string.Empty, options);
        var result = await instance.Result;
        if (result is { Data: CvExperienceInfo experience })
        {
            Editor.EditDone();
            experience.CvId = Cv.Id;
            Editor.CvExperiences.Add(experience);
            StateHasChanged();
        }
    }

    private void RemoveExperience(CvExperienceInfo experience)
    {
        Editor.CvExperiences.Remove(experience);
        Editor.EditDone();
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        Editor.CvExperiences = db.CvExperiences.AsNoTracking().Where(x => x.CvId == Cv.Id).ToList();
        await db.DisposeAsync();
        StateHasChanged();
    }
}