using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using Skills.Components.Components;
using Skills.Components.Dialogs;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Models.CV;
using Skills.Models.Enums;
using Skills.Services;

namespace Skills.Components.Pages.Profile;

public partial class ProfileCv : FullComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Parameter] public UserModel User { get; set; } = null!;
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public IConfiguration Configuration { get; set; } = null!;
    [Inject] public WordExportService WordExportService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    
    private List<BreadcrumbItem> _breadcrumbs = [];

    private List<CvInfo> _cvs = new();

    private UserModel _currentUser = null!;
    private bool _sortMostRecent = false;
    private string SortActionText => _sortMostRecent ? "Du plus récent au plus ancien" : "Du plus ancien au plus récent";
    private string SortActionIcon => _sortMostRecent ? "fas fa-arrow-down-1-9" : "fas fa-arrow-up-9-1";

    private bool _loading;

    protected override async Task OnInitializedAsync()
    {
        _breadcrumbs.Add(new BreadcrumbItem("Accueil", "/"));
        _breadcrumbs.Add(new BreadcrumbItem("Utilisateurs", "/users"));
        _breadcrumbs.Add(new BreadcrumbItem(User.Name, $"/overview/{User.Username}"));
        _breadcrumbs.Add(new BreadcrumbItem("CV", null, true));
        
        await RefreshDataAsync();
        StateHasChanged();

        var state = await AuthenticationState;
        var username = state.User.FindFirstValue("username");
        await using var db = await Factory.CreateDbContextAsync();
        _currentUser = db.Users.FirstOrDefault(x => x.Username == username)!;
    }

    private async Task CreateCvAsync()
    {
        await using var db = await Factory.CreateDbContextAsync();
        var cv = new CvInfo
        {
            UserId = User.Id,
            Title = $"CV{(db.CVs.AsNoTracking().Any(x => x.UserId == User.Id) ? $" ({db.CVs.AsNoTracking().Count(x => x.UserId == User.Id)})" : string.Empty)}",
            Job = User.Job,
            PhoneNumber = Configuration.GetSection("CV")["ManagerPhoneNumber"] ?? User.PhoneNumber
        };
        if (User.BirthDate.HasValue) cv.BirthDate = User.BirthDate.Value;
        
        db.CVs.Add(cv);
        db.Notifications.Add(new UserNotification
        {
            RecipientId = User.Id,
            SenderId = _currentUser.Id,
            Severity = NotificationSeverity.Hint,
            Content = $"{_currentUser.Name} a créé un CV sur votre profil.\nAllez le consulter !"
        });
        await db.SaveChangesAsync();
        
        await SendUpdateAsync();
        await SendNotificationUpdateAsync(User.Username);
        
        NavManager.NavigateTo($"/overview/{User.Username}/cv-editor/{cv.Id}");
    }

    private async Task DuplicateCvAsync(CvInfo cv)
    {
        await using var db = await Factory.CreateDbContextAsync();
        var copy = new CvInfo
        {
            UserId = cv.UserId,
            PhoneNumber = cv.PhoneNumber,
            Title = $"{cv.Title} (Copie)",
            BirthDate = cv.BirthDate,
            Job = cv.Job,
            MinLevel = cv.MinLevel
        };

        var educations = cv.Educations.Select(x => new CvEducationInfo { CvId = copy.Id, EducationId = x.Id }).ToList();
        var certifications = cv.Certifications.Select(x => new CvCertificationInfo() { CvId = copy.Id, CertificationId = x.Id }).ToList();
        var experiences = cv.Experiences.Select(x => new CvExperienceInfo() { CvId = copy.Id, ExperienceId = x.Id }).ToList();
        var skills = cv.Skills.Select(x => new CvSkillInfo { CvId = copy.Id, SkillId = x.Id, IsSoftSkill = x.IsSoftSkill }).ToList();

        db.CVs.Add(copy);
        db.CvEducations.AddRange(educations);
        db.CvCertifications.AddRange(certifications);
        db.CvExperiences.AddRange(experiences);
        db.CvSkills.AddRange(skills);
        await db.SaveChangesAsync();
        
        await RefreshDataAsync();
        StateHasChanged();

        await SendUpdateAsync();
    }
    
    private async Task DeleteCvAsync(CvInfo cv)
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, $"Voulez-vous vraiment supprimer ce CV ? \nCette action est irréversible !" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            await using var db = await Factory.CreateDbContextAsync();
            var oldCv = await db.CVs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == cv.Id);
            if (oldCv != null)
            {
                db.CVs.Remove(oldCv);
                await db.SaveChangesAsync();
            }
           
            await RefreshDataAsync();
            StateHasChanged();
            
            await SendUpdateAsync();
        }
    }

    private async Task ExportCvAsync(CvInfo cv)
    {
        _loading = true;
        StateHasChanged();
        var transaction = await WordExportService.ExportCvAsync<MemoryStream>(cv);
        if (transaction.State == ImportState.Successful)
        {
            var streamRef = new DotNetStreamReference(transaction.Value!);
            await JsRuntime.InvokeVoidAsync("downloadFileFromStream", $"CV_{User.Username}.docx", streamRef);
            Snackbar.Add($"CV de {User.Name} exporté avec succès !", Severity.Success);
        }
        else if (transaction.State == ImportState.Skipped)
        {
            Snackbar.Add($"{transaction.Message}", Severity.Warning);
        }
        else if(transaction.State == ImportState.Crashed)
        {
            Snackbar.Add($"{transaction.Message}", Severity.Error);
            Snackbar.Add($"{transaction.ErrorMessage}", Severity.Error);
        }

        _loading = false;
        StateHasChanged();
    }

    private void OnSortChanged()
    {
        _sortMostRecent = !_sortMostRecent;
        if (_sortMostRecent) _cvs = _cvs.OrderByDescending(x => x.CreatedAt).ToList();
        else _cvs = _cvs.OrderBy(x => x.CreatedAt).ToList();
        StateHasChanged();
    }

    public override async Task RefreshDataAsync()
    {
        await using var db = await Factory.CreateDbContextAsync();
        _cvs = db.CVs.AsNoTracking()
                     .Include(x => x.Skills).ThenInclude(x => x.Skill)
                     .Include(x => x.Educations).Include(x => x.Experiences)
                     .Include(x => x.Certifications)
                     .AsSplitQuery()
                     .OrderByDescending(x => x.CreatedAt)
                     .ToList();
        
        OnSortChanged();
    }
}