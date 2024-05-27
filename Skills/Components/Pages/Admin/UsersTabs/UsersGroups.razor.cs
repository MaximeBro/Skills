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

public partial class UsersGroups : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Parameter] public UsersManagement Manager { get; set; } = null!;

    private List<GroupModel> _groups = new();
    private MudTextField<string> _groupInput = null!;
    private SingleStringModel _inputModel = new();

    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
    }

    private async Task AddAsync()
    {
        if (!string.IsNullOrWhiteSpace(_inputModel.Value))
        {
            var group = new GroupModel
            {
                Name = _inputModel.Value
            };

            await  _groupInput.Clear();
            var db = await Factory.CreateDbContextAsync();
            db.Groups.Add(group);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }
    
    private async Task EditAsync(GroupModel group)
    {
        var parameters = new DialogParameters<GroupDialog> { { x => x.Name, group.Name} };
        var instance = await DialogService.ShowAsync<GroupDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: GroupModel groupModel })
        {
            var db = await Factory.CreateDbContextAsync();
            var oldGroup = db.Groups.AsNoTracking().FirstOrDefault(x => x.Id == group.Id);
            if (oldGroup != null)
            {
                oldGroup.Name = groupModel.Name;
                oldGroup.Icon = groupModel.Icon;
                
                db.Update(oldGroup);
                await db.SaveChangesAsync();
            }
            else
            {
                Snackbar.Add("Le groupe est introuvable ! Il a peut être été modifié ou supprimé par un autre utilisateur entre temps.", Severity.Error);
                return;
            }

            await db.DisposeAsync();
            await RefreshDataAsync();
            await Manager.RefreshUsersAsync();
        }
    }
    
    private async Task DeleteAsync(GroupModel group)
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, $"Voulez-vous vraiment supprimer le groupe {group.Name} ? Cette action est irréversible !" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            var db = await Factory.CreateDbContextAsync();
            var users = await db.Users.AsTracking().Where(x => x.GroupId == group.Id).ToListAsync();
            foreach (var user in users) user.GroupId = null;
            db.Users.UpdateRange(users);
            db.Groups.Remove(group);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
            await Manager.RefreshUsersAsync();
        }
    }

    protected override async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _groups = await db.Groups.AsNoTracking().ToListAsync();
    }
}