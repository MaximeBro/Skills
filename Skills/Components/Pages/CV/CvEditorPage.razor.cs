using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
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
    [Inject] public ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public string Username { get; set; } = null!;
    [Parameter] public Guid CvId { get; set; }

    private List<BreadcrumbItem> _breadcrumbs = new();
    private UserModel _user = null!;
    private CvInfo _cv = null!;
    private DateTime? _birthDate;

    private CvEditorPage_Education _education = null!;
    private CvEditorPage_Certification _certification = null!;
    private CvEditorPage_SafetyCertification _safetyCertification = null!;
    private CvEditorPage_Skills _skills = null!;
    private CvEditorPage_Experiences _experiences = null!;
    
    protected override async Task OnInitializedAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        var user = db.Users.AsNoTracking().FirstOrDefault(x => x.Username == Username);
        var cv = db.CVs.AsNoTracking()
                       .Include(x => x.Education)
                       .Include(x => x.Certifications)
                       .Include(x => x.SafetyCertifications)
                       .Include(x => x.Skills)
                       .FirstOrDefault(x => x.Id == CvId); // Imports all cv data from other tables
        
        if (user is null || cv is null)
        {
            NavManager.NavigateTo("/", true);
            return;
        }
        
        _user = user;
        _cv = cv;
        _birthDate = _cv.BirthDate == DateTime.MinValue ? null : _cv.BirthDate;
        
        _breadcrumbs.Add(new BreadcrumbItem("Accueil", "/"));
        _breadcrumbs.Add(new BreadcrumbItem("Utilisateurs", "/users"));
        _breadcrumbs.Add(new BreadcrumbItem(user.Name, $"/overview/{Username}"));
        _breadcrumbs.Add(new BreadcrumbItem("CV", $"/overview/{Username}/1"));
        _breadcrumbs.Add(new BreadcrumbItem(_cv.Title, null, true));
        await db.DisposeAsync();
        await RefreshDataAsync();
    }

    private async Task SaveDataAsync()
    {
        // Adds new cv fields
        var db = await Factory.CreateDbContextAsync();
        var educationsToAdd = _education.CvEducations.Where(x => !db.CvEducations.AsNoTracking().Select(y => y.Id).Contains(x.Id)).ToList();
        var certificationsToAdd = _certification.CvCertifications.Where(x => !db.CvCertifications.AsNoTracking().Select(y => y.Id).Contains(x.Id)).ToList();
        var experiencesToAdd = _experiences.CvExperiences.Where(x => !db.CvExperiences.AsNoTracking().Select(y => y.Id).Contains(x.Id)).ToList();

        List<CvSafetyCertificationInfo> safetyCerts = [];
        foreach (var cert in _safetyCertification.HeldCertifications.Where(x => x.Value))
        {
            if (db.SafetyCertifications.AsNoTracking().Any(x => x.Id == cert.Key))
            {
                safetyCerts.Add(new CvSafetyCertificationInfo { CvId = CvId, CertId = cert.Key });
            }
        }

        var safetyCertsToAdd = safetyCerts.Where(x => !db.CvSafetyCertifications.AsNoTracking().Select(y => y.Id).Contains(x.Id)).ToList();

        var heldSkills = _skills.ChosenSkills.Where(x => x.Value).Select(x => new CvSkillInfo
        {
            CvId = CvId,
            SkillId = x.Key.Id,
            IsSoftSkill = x.Key.Type?.ToLower() == "soft-skill"
        }).ToList();
        var skillsToAdd = heldSkills.Where(x => !db.CvSkills.AsNoTracking().Select(y => y.SkillId).Contains(x.SkillId)).ToList();

        db.CvEducations.AddRange(educationsToAdd);
        db.CvCertifications.AddRange(certificationsToAdd);
        db.CvExperiences.AddRange(experiencesToAdd);
        db.CvSafetyCertifications.AddRange(safetyCertsToAdd);
        db.CvSkills.AddRange(skillsToAdd);
        await db.SaveChangesAsync();
        await db.DisposeAsync();


        // Removes old cv fields
        db = await Factory.CreateDbContextAsync();
        var educationsToDell = db.CvEducations.AsNoTracking().Where(x => !_education.CvEducations.Select(y => y.Id).Contains(x.Id)).ToList();
        var certificationsToDell = db.CvCertifications.AsNoTracking().Where(x => !_certification.CvCertifications.Select(y => y.Id).Contains(x.Id)).ToList();
        var experiencesToDell = db.CvExperiences.AsNoTracking().Where(x => !_experiences.CvExperiences.Select(y => y.Id).Contains(x.Id)).ToList();
        var safetyCertsToDell = db.CvSafetyCertifications.AsNoTracking().Where(x => !safetyCerts.Select(y => y.CertId).Contains(x.CertId)).ToList();
        var skillsIds = _skills.ChosenSkills.Where(y => y.Value).Select(y => y.Key.Id).ToList();
        var skillsToDell = db.CvSkills.AsNoTracking().Where(x => !skillsIds.Contains(x.SkillId)).ToList();

        db.CvEducations.RemoveRange(educationsToDell);
        db.CvCertifications.RemoveRange(certificationsToDell);
        db.CvExperiences.RemoveRange(experiencesToDell);
        db.CvSafetyCertifications.RemoveRange(safetyCertsToDell);
        db.CvSkills.RemoveRange(skillsToDell);
        await db.SaveChangesAsync();

        _cv.BirthDate = _birthDate ?? _cv.BirthDate;
        db.CVs.Update(_cv);

        await db.SaveChangesAsync();
        await db.DisposeAsync();
        Snackbar.Configuration.BackgroundBlurred = true;
        Snackbar.Add("Modifications enregistrées !", Severity.Success, options =>
        {
            options.ShowCloseIcon = false;
            options.HideTransitionDuration = 1500;
        });
        await RefreshDataAsync();
    }

    private async Task RefreshDataAsync()
    {
        StateHasChanged();
    }
}