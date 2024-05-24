using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Components.Dialogs.CV;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models.CV;
using Skills.Models.Overview;

namespace Skills.Components.Pages.CV;

public partial class CvEditorPage_Certification : FullComponentBase
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
    
    private void OnCheckChanged(bool value, UserCertificationInfo cert)
    {
        Editor.CvCertifications[cert] = value;
        _selectAll = Editor.CvCertifications.All(x => x.Value);
        Editor.EditDone();
    }
    
    private void SelectAllChanged(bool value)
    {
        foreach (var key in Editor.CvCertifications.Keys)
        {
            Editor.CvCertifications[key] = value;
        }
        _selectAll = Editor.CvCertifications.All(x => x.Value);
        Editor.EditDone();
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        var userCertifications = await db.UserCertifications.AsNoTracking().Where(x => x.UserId == Cv.UserId).ToListAsync();
        var certifications = db.CvCertifications.AsNoTracking().Include(x => x.Certification)
            .Where(x => x.CvId == Cv.Id)
            .Select(x => x.CertificationId).ToList();
        
        Editor.CvCertifications.Clear();
        foreach (var certification in userCertifications)
        {
            Editor.CvCertifications.Add(certification, certifications.Contains(certification.Id));
        }
        
        _selectAll = Editor.CvCertifications.All(x => x.Value);
        await db.DisposeAsync();
    }
}