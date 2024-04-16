using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Models.CV;

namespace Skills.Components.Pages.CV;

public partial class CvEditorPage_SafetyCertification : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;

    [Parameter] public CvInfo Cv { get; set; } = null!;

    public List<CvSafetyCertificationInfo> CvSafetyCertifications = new();
    public List<SafetyCertification> SafetyCertifications = new();
    private Dictionary<string, List<SafetyCertification>> _groupedCertifications = new();
    public Dictionary<Guid, bool> HeldCertifications = new();

    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        CvSafetyCertifications = db.CvSafetyCertifications.AsNoTracking().Where(x => x.CvId == Cv.Id).Include(x => x.Certification).ToList();
        SafetyCertifications = db.SafetyCertifications.AsNoTracking().ToList();
        _groupedCertifications = SafetyCertifications.GroupBy(x => x.Category).ToDictionary(x => x.Key, y => y.ToList());
        foreach (var certification in SafetyCertifications) HeldCertifications.Add(certification.Id, CvSafetyCertifications.Select(x => x.Id).Contains(certification.Id));
        await db.DisposeAsync();
        StateHasChanged();
    }
}