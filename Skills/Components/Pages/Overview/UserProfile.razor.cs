using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Dialogs.Users;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Models.Enums;

namespace Skills.Components.Pages.Overview;

public partial class UserProfile
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Parameter] public UserModel User { get; set; } = null!;

    private string _name = string.Empty;
    private string _email = string.Empty;
    private string _policy = string.Empty;
    
    private DialogOptions _options = new DialogOptions
    {
        ClassBackground = "chrome-bg",
        CloseOnEscapeKey = true,
        DisableBackdropClick = false,
        CloseButton = true,
        NoHeader = true
    };

    protected override void OnInitialized()
    {
        _name = User.Name;
        _email = User.Email;
        _policy = string.Join(", ", User.Policy.GetFlags());
    }

    private async Task EditNameAsync()
    {
        var parameters = new DialogParameters<EditNameDialog> { { x => x.Name, _name } };
        var instance = await DialogService.ShowAsync<EditNameDialog>(string.Empty, parameters, _options);
        var result = await instance.Result;
        if (result is { Data: string name } && !string.IsNullOrWhiteSpace(name))
        {
            var db = await Factory.CreateDbContextAsync();

            var user = await db.Users.AsTracking().FirstOrDefaultAsync(x => x.Id == User.Id);
            if (user != null) user.Name = name;

            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }

    private async Task EditEmailAsync()
    {
        var parameters = new DialogParameters<EditEmailDialog> { { x => x.Email, _email } };
        var instance = await DialogService.ShowAsync<EditEmailDialog>(string.Empty, parameters, _options);
        var result = await instance.Result;
        if (result is { Data: string email } && !string.IsNullOrWhiteSpace(email))
        {
            var db = await Factory.CreateDbContextAsync();

            var alreadyExist = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email) != null;
            if (!alreadyExist)
            {
                var user = await db.Users.AsTracking().FirstOrDefaultAsync(x => x.Id == User.Id);
                if (user != null) user.Email = email;
            }
            else
            {
                Snackbar.Add("Cette adresse email est déjà utilisée !", Severity.Error);
            }
            
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }
    
    private async Task EditPolicyAsync()
    {
        var parameters = new DialogParameters<EditPolicyDialog> { { x => x.Policy, User.Policy } };
        var instance = await DialogService.ShowAsync<EditPolicyDialog>(string.Empty, parameters, _options);
        var result = await instance.Result;

        if (result is { Data: IEnumerable<PermissionPolicy> policies })
        {
            var db = await Factory.CreateDbContextAsync();

            var user = await db.Users.AsTracking().FirstOrDefaultAsync(x => x.Id == User.Id);
            if (user != null) user.Policy = policies.Aggregate((current, next) => current | next); // aggregates flags to a single one

            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == User.Id);
        await db.DisposeAsync();
        if (user is null)
        {
            NavManager.NavigateTo("/", true);
            return;
        }
        
        User = user;
        _name = User.Name;
        _email = User.Email;
        _policy = string.Join(", ", User.Policy.GetFlags());
        StateHasChanged();
    }
}