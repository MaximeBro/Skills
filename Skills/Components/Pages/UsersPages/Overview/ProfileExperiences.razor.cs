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

public partial class ProfileExperiences : FullComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Parameter] public UserModel User { get; set; } = null!;
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public ADAuthenticationService AuthenticationService { get; set; } = null!;
    
    private List<BreadcrumbItem> _breadcrumbs = [];

    private List<UserExperienceInfo> _experiences = [];
    
    private bool _sortMostRecent = false;
    private string SortActionText => _sortMostRecent ? "Du plus récent au plus ancien" : "Du plus ancien au plus récent";
    private string SortActionIcon => _sortMostRecent ? "fas fa-arrow-up-9-1" : "fas fa-arrow-down-1-9";
    
    protected override async Task OnInitializedAsync()
    {
        _breadcrumbs.Add(new BreadcrumbItem("Accueil", "/"));
        _breadcrumbs.Add(new BreadcrumbItem("Utilisateurs", "/users"));
        _breadcrumbs.Add(new BreadcrumbItem(User.Name, $"/overview/{User.Username}"));
        _breadcrumbs.Add(new BreadcrumbItem("Expériences", null, true));

        await RefreshDataAsync();
        StateHasChanged();
    }
    
    private async Task CreateExperienceAsync()
    {
        var authorized = await CheckPermissionsAsync();
        if (authorized)
        {
            var options = Hardcoded.DialogOptions;
            options.MaxWidth = MaxWidth.Medium;
            var instance = await DialogService.ShowAsync<ExperienceDialog>(string.Empty, options);
            var result = await instance.Result;
            if (result is { Data: UserExperienceInfo experience })
            {
                experience.UserId = User.Id;
                var db = await Factory.CreateDbContextAsync();
                db.UserExperiences.Add(experience);
                await db.SaveChangesAsync();
                await db.DisposeAsync();
                
                await RefreshDataAsync();
                StateHasChanged();
                
                await SendUpdateAsync();
            }
        }
    }
    
    private async Task EditExperienceAsync(UserExperienceInfo experience)
    {
        var authorized = await CheckPermissionsAsync();
        if (authorized)
        {
            var parameters = new DialogParameters<ExperienceDialog> { { x=> x.Experience, experience} };
            var options = Hardcoded.DialogOptions;
            options.MaxWidth = MaxWidth.Medium;
            var instance = await DialogService.ShowAsync<ExperienceDialog>(string.Empty, parameters, options);
            var result = await instance.Result;
            if (result is { Data: UserExperienceInfo newExperience })
            {
                var db = await Factory.CreateDbContextAsync();
                experience.Title = newExperience.Title;
                experience.Category = newExperience.Category;
                experience.StartsAt = newExperience.StartsAt;
                experience.EndsAt = newExperience.EndsAt;
                experience.Description = newExperience.Description;
                db.UserExperiences.Update(experience);
                await db.SaveChangesAsync();
                await db.DisposeAsync();
                
                await RefreshDataAsync();
                StateHasChanged();
                
                await SendUpdateAsync();
            }
        }
    }
    
    private async Task DeleteExperienceAsync(UserExperienceInfo experience)
    {
        var authorized = await CheckPermissionsAsync();
        if (authorized)
        {
            var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, $"Voulez-vous vraiment supprimer cette expérience ? Cette action est irréversible !" } };
            var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
            var result = await instance.Result;
            if (result.Data != null && (bool)result.Data)
            {
                var db = await Factory.CreateDbContextAsync();
                db.UserExperiences.Remove(experience);
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
        if (_sortMostRecent) _experiences = _experiences.OrderByDescending(x => x.EndsAt).ToList();
        else _experiences = _experiences.OrderBy(x => x.EndsAt).ToList();
        StateHasChanged();
    }
    
    public override async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _experiences = await db.UserExperiences.AsNoTracking().Where(x => x.UserId == User.Id).ToListAsync();
        OnSortChanged();
        await db.DisposeAsync();
    }
}