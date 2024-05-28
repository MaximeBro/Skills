using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using Skills.Components.Components;
using Skills.Components.Pages.UsersPages.Overview;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Models.CV;
using Skills.Models.Overview;

namespace Skills.Components.Pages.CV;

public partial class CvEditorPage : FullComponentBase, IAsyncDisposable
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter] public string Username { get; set; } = null!;
    [Parameter] public Guid CvId { get; set; }

    private readonly List<BreadcrumbItem> _breadcrumbs = new();
    private UserModel _user = null!;
    private CvInfo _cv = null!;
    private DateTime? _birthDate;

    private CvEditorPage_Education _education = null!;
    private CvEditorPage_Certification _certification = null!;
    private CvEditorPage_SafetyCertification _safetyCertification = null!;
    private CvEditorPage_Skills _skills = null!;
    private CvEditorPage_Experiences _experiences = null!;

    public Dictionary<UserEducationInfo, bool> CvEducations { get; set; } = new();
    public Dictionary<UserCertificationInfo, bool> CvCertifications { get; set; } = new();
    public Dictionary<UserExperienceInfo, bool> CvExperiences { get; set; } = new();
    public Dictionary<Guid, bool> HeldCertifications { get; set; } = new();
    public Dictionary<AbstractSkillModel, bool> ChosenSkills { get; set; } = new();

    private int _pendingEdits = 0;
    private bool _autoSave = true;

    private string _title = string.Empty;
    private string _job = string.Empty;
    private string _phoneNumber = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        var user = db.Users.AsNoTracking().FirstOrDefault(x => x.Username == Username);
        var cv = db.CVs.AsNoTracking().FirstOrDefault(x => x.Id == CvId);

        if (user is null || cv is null)
        {
            NavManager.NavigateTo("/", true);
            return;
        }

        _user = user;
        _cv = cv;
        _birthDate = _cv.BirthDate == DateTime.MinValue ? null : _cv.BirthDate;
        _title = _cv.Title;
        _job = _cv.Job;
        _phoneNumber = _cv.PhoneNumber;

        _breadcrumbs.Add(new BreadcrumbItem("Accueil", "/"));
        _breadcrumbs.Add(new BreadcrumbItem("Utilisateurs", "/users"));
        _breadcrumbs.Add(new BreadcrumbItem(user.Name, $"/overview/{Username}"));
        _breadcrumbs.Add(new BreadcrumbItem("CV", $"/overview/{Username}/5"));
        _breadcrumbs.Add(new BreadcrumbItem(_cv.Title, null, true));
        await db.DisposeAsync();
    }

    private async Task SaveDataAsync()
    {
        if (_pendingEdits == 0) return;
        
        // Adds new cv fields
        var db = await Factory.CreateDbContextAsync();


        var heldEducations = CvEducations.Where(x => x.Value).Select(x => new CvEducationInfo { EducationId = x.Key.Id, CvId = _cv.Id }).ToList();
        var educationsToAdd = heldEducations.Where(education => db.CvEducations.AsNoTracking().Where(x => x.CvId == _cv.Id).All(x => x.EducationId != education.EducationId)).ToList();
        
        var heldCertifications = CvCertifications.Where(x => x.Value).Select(x => new CvCertificationInfo() { CertificationId = x.Key.Id, CvId = _cv.Id }).ToList();
        var certificationsToAdd = heldCertifications.Where(certification => db.CvCertifications.AsNoTracking().Where(x => x.CvId == _cv.Id).All(x => x.CertificationId != certification.CertificationId)).ToList();
        
        var heldExperiences = CvExperiences.Where(x => x.Value).Select(x => new CvExperienceInfo() { ExperienceId = x.Key.Id, CvId = _cv.Id }).ToList();
        var experiencesToAdd = heldExperiences.Where(experience => db.CvExperiences.AsNoTracking().Where(x => x.CvId == _cv.Id).All(x => x.ExperienceId != experience.ExperienceId)).ToList();

        var heldSkills = ChosenSkills.Where(x => x.Value).Select(x => new CvSkillInfo
        {
            CvId = CvId,
            SkillId = x.Key.Id,
            IsSoftSkill = x.Key.Type?.ToLower() == "soft-skill"
        }).ToList();
        var skillsToAdd = heldSkills.Where(x => !db.CvSkills.AsNoTracking().Select(y => y.SkillId).Contains(x.SkillId)).ToList();
        
        List<CvSafetyCertificationInfo> safetyCerts = [];
        foreach (var cert in HeldCertifications.Where(x => x.Value))
        {
            if (db.SafetyCertifications.AsNoTracking().Any(x => x.Id == cert.Key))
            {
                safetyCerts.Add(new CvSafetyCertificationInfo { CvId = CvId, CertId = cert.Key });
            }
        }

        var safetyCertsToAdd = safetyCerts.Where(x => !db.CvSafetyCertifications.AsNoTracking().Select(y => y.Id).Contains(x.Id)).ToList();

        db.CvEducations.AddRange(educationsToAdd);
        db.CvCertifications.AddRange(certificationsToAdd);
        db.CvExperiences.AddRange(experiencesToAdd);
        db.CvSkills.AddRange(skillsToAdd);
        db.CvSafetyCertifications.AddRange(safetyCertsToAdd);
        

        // Removes old cv fields
        var safetyCertsToDell = db.CvSafetyCertifications.AsNoTracking().Where(x => !safetyCerts.Select(y => y.CertId).Contains(x.CertId)).ToList();
        var skillsIds = ChosenSkills.Where(x => x.Value).Select(x => x.Key.Id).ToList();
        var skillsToDell = db.CvSkills.AsNoTracking().Where(x => !skillsIds.Contains(x.SkillId)).ToList();

        var educationsIds = CvEducations.Where(x => x.Value).Select(x => x.Key.Id).ToList();
        var educationsToDell = db.CvEducations.AsNoTracking().Where(x => !educationsIds.Contains(x.EducationId)).ToList();

        var certificationsIds = CvCertifications.Where(x => x.Value).Select(x => x.Key.Id).ToList();
        var certificationsToDell = db.CvCertifications.AsNoTracking().Where(x => !certificationsIds.Contains(x.CertificationId)).ToList();

        var experiencesIds = CvExperiences.Where(x => x.Value).Select(x => x.Key.Id).ToList();
        var experiencesToDell = db.CvExperiences.AsNoTracking().Where(x => !experiencesIds.Contains(x.ExperienceId)).ToList();
        
        db.CvSafetyCertifications.RemoveRange(safetyCertsToDell);
        db.CvSkills.RemoveRange(skillsToDell);
        db.CvEducations.RemoveRange(educationsToDell);
        db.CvCertifications.RemoveRange(certificationsToDell);
        db.CvExperiences.RemoveRange(experiencesToDell);

        _cv.BirthDate = _birthDate ?? _cv.BirthDate;
        _cv.UpdatedAt = DateTime.Now;
        db.CVs.Update(_cv);

        await db.SaveChangesAsync();
        await db.DisposeAsync();
        Snackbar.Configuration.BackgroundBlurred = true;
        Snackbar.Add("Modifications enregistrées !", Severity.Success, options =>
        {
            options.ShowCloseIcon = false;
            options.HideTransitionDuration = 1500;
        });

        _pendingEdits = 0;
    }

    public void EditDone()
    {
        _pendingEdits++;
        StateHasChanged();
    }

    public override async ValueTask DisposeAsync()
    {
        if (_pendingEdits > 0 && _autoSave)
        {
            await Task.Run(async () => await SaveDataAsync());
        }
        await base.DisposeAsync();
    }

    private Task NavigateToProfileAsync()
    {
        Snackbar.Add("Un utilisateur vient de supprimer ce CV ! Redirection en cours...", Severity.Warning, Hardcoded.SnackbarOptions);
        NavManager.NavigateTo($"/overview/{Username}/1");
        return Task.CompletedTask;
    }
}