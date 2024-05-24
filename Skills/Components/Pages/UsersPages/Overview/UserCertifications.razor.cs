using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Components.Dialogs;
using Skills.Components.Dialogs.CV;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Models.Enums;
using Skills.Models.Overview;
using Skills.Services;

namespace Skills.Components.Pages.UsersPages.Overview;

public partial class UserCertifications : FullComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Parameter] public UserModel User { get; set; } = null!;
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public UserService UserService { get; set; } = null!;
    
    private List<BreadcrumbItem> _breadcrumbs = [];
    private List<UserCertificationInfo> _certifications = [];
    
    private bool _sortMostRecent = false;
    private string SortActionText => _sortMostRecent ? "Du plus récent au plus ancien" : "Du plus ancien au plus récent";
    private string SortActionIcon => _sortMostRecent ? "fas fa-arrow-up-9-1" : "fas fa-arrow-down-1-9";
    
    protected override async Task OnInitializedAsync()
    {
        _breadcrumbs.Add(new BreadcrumbItem("Accueil", "/"));
        _breadcrumbs.Add(new BreadcrumbItem("Utilisateurs", "/users"));
        _breadcrumbs.Add(new BreadcrumbItem(User.Name, $"/overview/{User.Username}"));
        _breadcrumbs.Add(new BreadcrumbItem("Diplômes", null, true));

        await RefreshDataAsync();
        StateHasChanged();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitSignalRAsync(nameof(UserCertifications), async() => await RefreshDataAsync());
        }
    }

    private async Task CreateCertificationAsync()
    {
        var authorized = await CheckPermissionsAsync();
        if (authorized)
        {
            var options = Hardcoded.DialogOptions;
            options.MaxWidth = MaxWidth.Medium;
            var instance = await DialogService.ShowAsync<CertificationDialog>(string.Empty, options);
            var result = await instance.Result;
            if (result is { Data: UserCertificationInfo certification })
            {
                certification.UserId = User.Id;
                var db = await Factory.CreateDbContextAsync();
                db.UserCertifications.Add(certification);
                await db.SaveChangesAsync();
                await db.DisposeAsync();

                await SendUpdateAsync(nameof(UserCertifications));
                await RefreshDataAsync();
                StateHasChanged();
            }
        }
    }

    private async Task EditCertificationAsync(UserCertificationInfo certification)
    {
        var authorized = await CheckPermissionsAsync();
        if (authorized)
        {
            var parameters = new DialogParameters<CertificationDialog> { { x=> x.Certification, certification} };
            var options = Hardcoded.DialogOptions;
            options.MaxWidth = MaxWidth.Medium;
            var instance = await DialogService.ShowAsync<CertificationDialog>(string.Empty, parameters, options);
            var result = await instance.Result;
            if (result is { Data: UserCertificationInfo newCertification })
            {
                var db = await Factory.CreateDbContextAsync();
                certification.Title = newCertification.Title;
                certification.Supplier = newCertification.Supplier;
                certification.Duration = newCertification.Duration;
                certification.Year = newCertification.Year;
                db.UserCertifications.Update(certification);
                await db.SaveChangesAsync();
                await db.DisposeAsync();

                await SendUpdateAsync(nameof(UserEducations));
                await RefreshDataAsync();
                StateHasChanged();
            }
        }
    }
    
    private async Task DeleteCertificationAsync(UserCertificationInfo certification)
    {
        var authorized = await CheckPermissionsAsync();
        if (authorized)
        {
            var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, $"Voulez-vous vraiment supprimer cette certification ? Cette action est irréversible !" } };
            var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
            var result = await instance.Result;
            if (result.Data != null && (bool)result.Data)
            {
                var db = await Factory.CreateDbContextAsync();
                db.UserCertifications.Remove(certification);
                await db.SaveChangesAsync();
                await db.DisposeAsync();

                await SendUpdateAsync(nameof(UserCertifications));
                await RefreshDataAsync();
                StateHasChanged();
            }
        }
    }
    
    private async Task<bool> CheckPermissionsAsync()
    {
        var state = await AuthenticationState;
        if (UserService.HasRequiredPermission(state, User, new[] { UserRole.Manager, UserRole.Admin }))
        {
            return true;
        }

        Snackbar.Add("Vous n'avez pas les permissions nécessaires pour effectuer des modifications sur le profil de cet utilisateur !", Severity.Error);
        return false;
    }

    private void OnSortChanged()
    {
        _sortMostRecent = !_sortMostRecent;
        if (_sortMostRecent) _certifications = _certifications.OrderByDescending(x => x.Year).ToList();
        else _certifications = _certifications.OrderBy(x => x.Year).ToList();
        StateHasChanged();
    }
    
    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _certifications = await db.UserCertifications.AsNoTracking().Where(x => x.UserId == User.Id).ToListAsync();
        OnSortChanged();
        await db.DisposeAsync();
    }
}