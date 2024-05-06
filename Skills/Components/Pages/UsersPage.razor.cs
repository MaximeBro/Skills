using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Databases;
using Skills.Models;
using Skills.Services;
using BreadcrumbItem = MudBlazor.BreadcrumbItem;
using Icons = MudBlazor.Icons.Material.Filled;

namespace Skills.Components.Pages;

public partial class UsersPage : ComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public ADAuthenticationService AuthenticationService { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;

    private List<BreadcrumbItem> _breadcrumbs = new();
    private List<UserModel> _users = new();
    private List<UserModel> _filteredUsers = new();
    private ClaimsPrincipal _user = null!;

    private bool _showDisabledAccounts = false;
    
    private string _search = string.Empty;
    private Func<UserModel, bool> QuickFilter => x =>
    {
        if (x.Name.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Username.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Email.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        
        return false;
    };

    private bool _tableView;
    private string _toggleIcon => _tableView ? Icons.TableChart : Icons.GridView;
    private string _toggleTooltip => _tableView ? "Tableau" : "Cartes";
    public Func<UserModel, string> DisabledColor => x => x.IsDisabled ? "color: orange;" : "color: inherit;";

    protected override async Task OnInitializedAsync()
    {
        _breadcrumbs.Add(new BreadcrumbItem("Accueil", "/"));
        _breadcrumbs.Add(new BreadcrumbItem("Utilisateurs", null, true));
        await RefreshDataAsync();
    }

    private void OnRowClicked(DataGridRowClickEventArgs<UserModel> args)
    {
        if (args.MouseEventArgs.Detail == 2)
        {
            NavManager.NavigateTo($"/overview/{args.Item.Username}");
        }
    }

    private void ShowDisabledAccounts(bool? show)
    {
        if (show != null)
        {
            _showDisabledAccounts = (bool)show;
            _filteredUsers = _users.Where(x => !x.IsDisabled || x.IsDisabled == _showDisabledAccounts).ToList();
            StateHasChanged();
        }
    }
    
    private void NavigateToProfile(UserModel model) => NavManager.NavigateTo($"/overview/{model.Username}");

    private async Task RefreshDataAsync()
    {
        var authState = await AuthenticationState;
        _user = authState.User;
        
        var db = await Factory.CreateDbContextAsync();
        _users = await db.Users.AsNoTracking().ToListAsync();
        _filteredUsers = _users.OrderBy(x => x.Name).Where(x => !x.IsDisabled || x.IsDisabled == _showDisabledAccounts).ToList();
        await db.DisposeAsync();
    }
}