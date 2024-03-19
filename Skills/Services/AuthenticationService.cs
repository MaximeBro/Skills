using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Skills.Components.Dialogs;

namespace Skills.Services;

public class AuthenticationService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IDialogService _dialogService;
    
    public AuthenticationService(AuthenticationStateProvider authenticationStateProvider, IDialogService dialogService)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _dialogService = dialogService;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.Identity is { IsAuthenticated: true };
    }

    public async Task<ClaimsPrincipal> GetUserAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User;
    }
    
    public async Task<IIdentity?> GetUserIdentityAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.Identity;
    }

    public async Task<bool> HasPermissionAsync()
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (!isAuthenticated)
        {
            return false;
        }

        // check user policy
        
        return true;
    }

    public async Task ShowLoginDialogAsync()
    {
        var options = new DialogOptions
        {
            ClassBackground = "chrome-bg",
            CloseOnEscapeKey = true,
            DisableBackdropClick = false,
            CloseButton = true,
            NoHeader = true
        };
        await _dialogService.ShowAsync<LoginRequiredDialog>(string.Empty, options);
    }
}