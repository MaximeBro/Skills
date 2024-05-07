using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Components.Dialogs;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Services;

namespace Skills.Components.Pages.Admin.UsersTabs;

public partial class UsersList : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public ActiveDirectoryService ADService { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Parameter] public UsersManagement Manager { get; set; } = null!;

    private MudDataGrid<UserModel> _grid = null!;
    private List<UserModel> _users = new();
    private List<GroupModel> _groups = new();
    private string _search = string.Empty;
    private bool _loading;
    private bool _pageLoading;

    public Func<UserModel, string> DisabledColor => x => x.IsDisabled ? "color: orange;" : "color: inherit;";
    
    public Func<UserModel, bool> QuickFilter => x =>
    {
        if (x.Name.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Email.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Username.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Role.Humanize().Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;

        return false;
    };

    private async Task CreateUserAsync()
    {
        var options = Hardcoded.DialogOptions;
        options.MaxWidth = MaxWidth.ExtraLarge;
        var instance = await DialogService.ShowAsync<UserDialog>(string.Empty, options);
        var result = await instance.Result;
        if (result is { Data: UserModel model })
        {
            var db = await Factory.CreateDbContextAsync();
            var exists = db.Users.AsNoTracking().FirstOrDefault(x => x.Username == model.Username) != null;
            if (!exists)
            {
                db.Users.Add(model);
                await db.SaveChangesAsync();
            }
            else
            {
                Snackbar.Add("Un utilisateur existe déjà avec cet identifiant !", Severity.Error);
            }

            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }

    private async Task EditUserAsync(UserModel model)
    {
        var parameters = new DialogParameters<UserDialog> { { x => x.User, model } };
        var options = Hardcoded.DialogOptions;
        options.MaxWidth = MaxWidth.ExtraLarge;
        var instance = await DialogService.ShowAsync<UserDialog>(string.Empty, parameters, options);
        var result = await instance.Result;
        if (result is { Data: UserModel userModel })
        {
            var db = await Factory.CreateDbContextAsync();
            var old = db.Users.AsTracking().FirstOrDefault(x => x.Id == model.Id);
            if (old != null)
            {
                old.Name = userModel.Name;
                old.Email = userModel.Email;
                old.Username = userModel.Username;
                old.Role = userModel.Role;
                old.GroupId = userModel.GroupId;
                old.IsDisabled = userModel.IsDisabled;
                old.PhoneNumber = userModel.PhoneNumber;
                db.Users.Update(old);
                await db.SaveChangesAsync();
            }
            else
            {
                Snackbar.Add("Utilisateur introuvable ! Ce compte a peut être été modifié ou supprimé entre temps.", Severity.Error);
                return;
            }
            
            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }

    private async Task DeleteUserAsync(UserModel model)
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, $"Voulez-vous vraiment supprimer le compte {model.Name} ? Cette action est irréversible !" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            var db = await Factory.CreateDbContextAsync();
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == model.Username);
            if (user != null)
            {
                db.Users.Remove(user);
                await db.SaveChangesAsync();
            }
            await db.DisposeAsync();
            await RefreshDataAsync(); 
        }
    }

    private async Task OnRowClickedAsync(DataGridRowClickEventArgs<UserModel> args)
    {
        if (args.MouseEventArgs.Detail == 2)
        {
            await EditUserAsync(args.Item);
        }
    }

    private async Task ResyncUsersFromLocalADAsync()
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, "Cette action va récupérer tous les comptes utilisateurs de l'AD local et ajouter ceux qui manquent à la base de données de Skills. Voulez-vous effectuer cette action ?" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            _pageLoading = true;
            StateHasChanged();
            await ADService.CheckUsersAsync();
            await RefreshDataAsync();
            _pageLoading = false;
            StateHasChanged();
        }
    }

    private async Task PurgeUsersAsync()
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, "Cette action va supprimer tous les utilisateurs de la base. Ajoutez de nouveau un compte avant de vous déconnecter ou fermer le navigateur car votre session sera également supprimée pendant le processus. \nVoulez-vous vraiment effectuer cette action ?" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            _pageLoading = true;
            StateHasChanged();
            await ADService.PurgeUsersAsync();
            await RefreshDataAsync();
            _pageLoading = false;
            StateHasChanged();
        }
    }

    private async Task<GridData<UserModel>> GetUsersAsync(GridState<UserModel> state)
    {
        var db = await Factory.CreateDbContextAsync();
        _users = db.Users.AsNoTracking().Include(x => x.Group).Where(QuickFilter).OrderBy(x => x.IsDisabled).ToList();
        _groups = await db.Groups.AsNoTracking().ToListAsync();
        await db.DisposeAsync();

        return new GridData<UserModel>
        {
            Items = _users,
            TotalItems = _users.Count
        };
    }
    
    public async Task RefreshDataAsync()
    {
        _loading = true;
        await _grid.ReloadServerData();
        _loading = false;
    }
}