using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Dialogs;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Services;
using Icons = MudBlazor.Icons.Material.Filled;

namespace Skills.Components.Pages;

public partial class UsersPage
{
    [Inject] public AuthenticationService AuthenticationService { get; set; } = null!;
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;

    private List<UserModel> _users = new();
    private ClaimsPrincipal _user = null!;

    private string _search = string.Empty;
    private Func<UserModel, bool> QuickFilter => x =>
    {
        if (x.Name.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Username.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Email.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
            
        return false;
    };

    private int _toggleIndex = 1;
    private string _toggleIcon => _toggleIndex < 2 ? Icons.GridView : _toggleIndex == 2 ? Icons.ViewList : Icons.TableChart;
    private string _toggleTooltip => _toggleIndex < 2 ? "Cartes" : _toggleIndex == 2 ? "Liste" : "Tableau";
    private void ToggleView() => _toggleIndex += _toggleIndex == 3 ? -2 : 1;
    public Func<UserModel, string> DisabledColor => x => x.IsDisabled ? "color: orange;" : "color: inherit;";

    protected override async Task OnInitializedAsync()
    {
        await RefreshUsersAsync();
    }

    private async Task DeleteUserAsync(UserModel model)
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, $"Voulez-vous vraiment supprimer le compte {model.Name} ? Cette action est irréversible !" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            var db = await Factory.CreateDbContextAsync();
            db.Users.Remove(model);
            await db.SaveChangesAsync();
            await db.DisposeAsync();

            var email = _user.Identity?.Name ?? string.Empty;
            if (email == model.Email)
            {
                NavManager.NavigateTo("MicrosoftIdentity/Account/SignOut", true);
                return;
            }
            
            await RefreshUsersAsync();
            
        }
    }

    private void OnRowClicked(DataGridRowClickEventArgs<UserModel> args)
    {
        if (args.MouseEventArgs.Detail == 2)
        {
            NavManager.NavigateTo($"/overview/{args.Item.Username}");
        }
    }

    private void NavigateToProfile(UserModel model) => NavManager.NavigateTo($"/overview/{model.Username}");

    private async Task RefreshUsersAsync()
    {
        _user = await AuthenticationService.GetUserAsync();
        var db = await Factory.CreateDbContextAsync();
        _users = await db.Users.AsNoTracking().ToListAsync();
        await db.DisposeAsync();
    }
}