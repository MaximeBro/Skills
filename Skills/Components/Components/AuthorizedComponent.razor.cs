using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Skills.Components.Dialogs;
using Skills.Extensions;
using Skills.Models.Enums;
using Skills.Services;

namespace Skills.Components.Components;

public partial class AuthorizedComponent : FullComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!; 
    [Inject] public ADAuthenticationService AuthenticationService { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;

    [Parameter] public RenderFragment? Content { get; set; }
    [Parameter] public string? Roles { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        var authorized = await AuthenticationService.HasRequiredRoleAsync(AuthenticationState, UserRole.User);
        if(!authorized)
        {
            await ShowLoginDialogAsync();
        }

        AuthenticationService.OnNotAuthorized += ShowLoginDialogAsync;
    }
    
    /// <summary>
    /// In case the user is not authenticated and tries to access a restricted content, we show him this dialog.
    /// </summary>
    protected async Task ShowLoginDialogAsync()
    {
        var instance = await DialogService.ShowAsync<LoginRequiredDialog>(string.Empty, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is null or {Canceled: true} or { Data: false })
        {
            NavManager.NavigateTo("/dashboard", true);
        }
    }

    public void InvokeStateHasChanged() => StateHasChanged();
}