using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Models.CV;
using Skills.Models.Overview;

namespace Skills.Components.Pages.Profile;

public partial class ProfileSafetyCertifications : FullComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Parameter] public UserModel User { get; set; } = null!;
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    
    private List<BreadcrumbItem> _breadcrumbs = [];
    
    private List<SafetyCertification> _safetyCertifications = [];
    private List<UserSafetyCertificationInfo> _userSafetyCertifications = [];
    private Dictionary<string, List<SafetyCertification>> _groupedCertifications = [];
    
    protected override async Task OnInitializedAsync()
    {
        _breadcrumbs.Add(new BreadcrumbItem("Accueil", "/"));
        _breadcrumbs.Add(new BreadcrumbItem("Utilisateurs", "/users"));
        _breadcrumbs.Add(new BreadcrumbItem(User.Name, $"/overview/{User.Username}"));
        _breadcrumbs.Add(new BreadcrumbItem("Habilitations", null, true));

        await RefreshDataAsync();
        StateHasChanged();
    }

    private async Task OnCheckChangedAsync(bool value, Guid certificationId)
    {
        var db = await Factory.CreateDbContextAsync();
        var old = db.UserSafetyCertifications.FirstOrDefault(x => x.UserId == User.Id && x.CertId == certificationId);
        if (old != null)
        {
            db.UserSafetyCertifications.Remove(old);
        }
        else
        {
            db.UserSafetyCertifications.Add(new UserSafetyCertificationInfo
            {
                UserId = User.Id,
                CertId = certificationId
            });
        }

        await db.SaveChangesAsync();
        await db.DisposeAsync();
        await RefreshDataAsync();
        Snackbar.Add("Données sauvegardées !", Severity.Success, Hardcoded.SnackbarOptions);
        StateHasChanged();
    }
    
    public override async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _safetyCertifications = await db.SafetyCertifications.AsNoTracking().ToListAsync();
        _userSafetyCertifications = await db.UserSafetyCertifications.AsNoTracking().Where(x => x.UserId == User.Id).ToListAsync();
        _groupedCertifications = _safetyCertifications.GroupBy(x => x.Category).ToDictionary(x => x.Key, y => y.ToList());
        await db.DisposeAsync();
    }
}