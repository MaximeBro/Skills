using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Models.CV;
using Skills.Models.Overview;

namespace Skills.Components.Pages.CV;

public partial class CvEditorPage_SafetyCertification : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;

    
    [CascadingParameter(Name = "cv-editor")] public CvEditorPage Editor { get; set; } = null!;
    [Parameter] public CvInfo Cv { get; set; } = null!;

    private List<SafetyCertification> _safetyCertifications = [];
    private List<UserSafetyCertificationInfo> _userSafetyCertifications = [];
    private Dictionary<string, List<SafetyCertification>> _groupedCertifications = [];

    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
        StateHasChanged();
    }

    public override async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _safetyCertifications = await db.SafetyCertifications.AsNoTracking().ToListAsync();
        _userSafetyCertifications = await db.UserSafetyCertifications.AsNoTracking().Where(x => x.UserId == Cv.UserId).ToListAsync();
        _groupedCertifications = _safetyCertifications.GroupBy(x => x.Category).ToDictionary(x => x.Key, y => y.ToList());
        await db.DisposeAsync();
    }
}