using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Models;
using Skills.Models.CV;

namespace Skills.Components.Pages.CV;

public partial class CvEditorPage : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;

    [Parameter] public string Username { get; set; } = null!;
    [Parameter] public Guid CvId { get; set; }

    private UserModel _user = null!;
    private CvInfo _cv = null!;
    private DateTime? _birthDate;

    private CvEditorPage_Education _education = null!;
    private CvEditorPage_Certification _certification = null!;
    private CvEditorPage_SafetyCertification _safetyCertification = null!;
    private CvEditorPage_Skills _skills = null!;
    
    protected override async Task OnInitializedAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        var user = db.Users.AsNoTracking().FirstOrDefault(x => x.Username == Username);
        var cv = db.CVs.AsNoTracking()
                       .Include(x => x.Education)
                       .Include(x => x.Certifications)
                       .Include(x => x.SafetyCertifications)
                       .Include(x => x.Skills).Include(x => x.SoftSkills)
                       .FirstOrDefault(x => x.Id == CvId); // Imports all cv's data from other tables
        
        if (user is null || cv is null)
        {
            NavManager.NavigateTo("/", true);
            return;
        }
        
        _user = user;
        _cv = cv;
        _birthDate = _cv.BirthDate == DateTime.MinValue ? null : _cv.BirthDate;
        await db.DisposeAsync();
        await RefreshDataAsync();
    }

    private async Task SaveDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        var educationsToAdd = _education.CvEducations.Where(x => !db.CvEducations.AsNoTracking().Select(y => y.Id).Contains(x.Id)).ToList();
        var certificationsToAdd = _certification.CvCertifications.Where(x => !db.CvCertifications.AsNoTracking().Select(y => y.Id).Contains(x.Id)).ToList();

        db.CvEducations.AddRange(educationsToAdd);
        db.CvCertifications.AddRange(certificationsToAdd);
        
        await db.SaveChangesAsync();
        await db.DisposeAsync();
    }
    
    private async Task RefreshDataAsync()
    {
        StateHasChanged();
    }
}