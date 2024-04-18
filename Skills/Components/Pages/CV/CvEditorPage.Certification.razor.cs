using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Components.Dialogs.CV;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models.CV;

namespace Skills.Components.Pages.CV;

public partial class CvEditorPage_Certification : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;

    [Parameter] public CvInfo Cv { get; set; } = null!;
    
    public List<CvCertificationInfo> CvCertifications = new();

    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
    }

    private async Task CreateCertificationAsync()
    {
        var options = Hardcoded.DialogOptions;
        options.MaxWidth = MaxWidth.Medium;
        var instance = await DialogService.ShowAsync<CertificationDialog>(string.Empty, options);
        var result = await instance.Result;
        if (result is { Data: CvCertificationInfo certification })
        {
            certification.CvId = Cv.Id;
            CvCertifications.Add(certification);
            StateHasChanged();
        }
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        CvCertifications = db.CvCertifications.AsNoTracking().Where(x => x.CvId == Cv.Id).ToList();
        await db.DisposeAsync();
    }
}