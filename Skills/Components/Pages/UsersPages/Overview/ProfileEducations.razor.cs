using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
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

public partial class ProfileEducations : FullComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Parameter] public UserModel User { get; set; } = null!;
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public ADAuthenticationService AuthenticationService { get; set; } = null!;
    
    private List<BreadcrumbItem> _breadcrumbs = [];

    private List<UserEducationInfo> _educations = [];
    
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

    private async Task CreateEducationAsync()
    {
        var authorized = await CheckPermissionsAsync();
        if (authorized)
        {
            var options = Hardcoded.DialogOptions;
            options.MaxWidth = MaxWidth.Medium;
            var instance = await DialogService.ShowAsync<EducationDialog>(string.Empty, options);
            var result = await instance.Result;
            if (result is { Data: UserEducationInfo education })
            {
                education.UserId = User.Id;
                var db = await Factory.CreateDbContextAsync();
                db.UserEducations.Add(education);
                await db.SaveChangesAsync();
                await db.DisposeAsync();
                
                await RefreshDataAsync();
                StateHasChanged();
                
                await SendUpdateAsync();
            }
        }
    }
    
    private async Task EditEducationAsync(UserEducationInfo education)
    {
        var authorized = await CheckPermissionsAsync();
        if (authorized)
        {
            var parameters = new DialogParameters<EducationDialog> { { x=> x.Education, education} };
            var options = Hardcoded.DialogOptions;
            options.MaxWidth = MaxWidth.Medium;
            var instance = await DialogService.ShowAsync<EducationDialog>(string.Empty, parameters, options);
            var result = await instance.Result;
            if (result is { Data: UserEducationInfo newEducation })
            {
                var db = await Factory.CreateDbContextAsync();
                education.YearStart = newEducation.YearStart;
                education.YearEnd = newEducation.YearEnd;
                education.Title = newEducation.Title;
                education.Supplier = newEducation.Supplier;
                education.Town = newEducation.Town;
                education.Description = newEducation.Description;
                db.UserEducations.Update(education);
                await db.SaveChangesAsync();
                await db.DisposeAsync();
                
                await RefreshDataAsync();
                StateHasChanged();
                
                await SendUpdateAsync();
            }
        }
    }

    private async Task DeleteEducationAsync(UserEducationInfo education)
    {
        var authorized = await CheckPermissionsAsync();
        if (authorized)
        {
            var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, $"Voulez-vous vraiment supprimer ce diplôme ? Cette action est irréversible !" } };
            var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
            var result = await instance.Result;
            if (result.Data != null && (bool)result.Data)
            {
                var db = await Factory.CreateDbContextAsync();
                db.UserEducations.Remove(education);
                await db.SaveChangesAsync();
                await db.DisposeAsync();
                
                await RefreshDataAsync();
                StateHasChanged();
                
                await SendUpdateAsync();
            }
        }
    }
    
    private async Task<bool> CheckPermissionsAsync()
    {
        var authorized = await AuthenticationService.HasRequiredRoleAsync(AuthenticationState, new[] { UserRole.Manager.ToString(), UserRole.Admin.ToString() });
        if (authorized)
        {
            return true;
        }

        Snackbar.Add("Vous n'avez pas les permissions nécessaires pour effectuer des modifications sur le profil de cet utilisateur !", Severity.Error);
        return false;
    }
    
    private void OnSortChanged()
    {
        _sortMostRecent = !_sortMostRecent;
        if (_sortMostRecent) _educations = _educations.OrderByDescending(x => x.YearEnd).ToList();
        else _educations = _educations.OrderBy(x => x.YearEnd).ToList();
        StateHasChanged();
    }
    
    public override async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _educations = await db.UserEducations.AsNoTracking().Where(x => x.UserId == User.Id).ToListAsync();
        OnSortChanged();
        await db.DisposeAsync();
    }
}