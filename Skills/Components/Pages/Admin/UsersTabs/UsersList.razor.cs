using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Dialogs;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;

namespace Skills.Components.Pages.Admin.UsersTabs;

public partial class UsersList : ComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Parameter] public UsersManagement Manager { get; set; } = null!;

    private MudDataGrid<UserModel> _grid = null!;
    private List<UserModel> _users = new();
    private List<GroupModel> _groups = new();
    private string _search = string.Empty;

    public Func<UserModel, string> DisabledColor => x => x.IsDisabled ? "color: orange;" : "color: inherit;";
    
    public Func<UserModel, bool> QuickFilter => x =>
    {
        if (x.Name.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Email.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Username.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Role.Humanize().Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;

        return false;
    };

    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
    }

    private async Task CreateUserAsync()
    {
        var instance = await DialogService.ShowAsync<CreateUserDialog>(string.Empty, Hardcoded.DialogOptions);
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
    
    private async Task OnEditDoneAsync(UserModel model)
    {
        var db = await Factory.CreateDbContextAsync();
        db.Users.Update(model);
        await db.SaveChangesAsync();
        await db.DisposeAsync();
        await RefreshDataAsync();
    }

    public async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _users = await db.Users.AsNoTracking().Include(x => x.Group).ToListAsync();
        _groups = await db.Groups.AsNoTracking().ToListAsync();
        await db.DisposeAsync();
    }
}