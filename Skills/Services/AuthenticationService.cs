using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Skills.Components.Dialogs;
using Skills.Extensions;

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

    /// <summary>
    /// Checks if the user is really authenticated with its identity state.
    /// </summary>
    /// <returns>True if authenticated, false it not.</returns>
    public async Task<bool> IsAuthenticatedAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.Identity is { IsAuthenticated: true };
    }

    
    /// <summary>
    /// Gets the current authenticated user state.
    /// </summary>
    /// <returns>The user as a <see cref="ClaimsPrincipal"/>.</returns>
    public async Task<ClaimsPrincipal> GetUserAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User;
    }

    /// <summary>
    /// Retrieves the user Azure AD identity.
    /// </summary>
    /// <returns>The user information as a <see cref="IIdentity"/>.</returns>
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

    /// <summary>
    /// In case the user is not authenticated and tries to access a restricted content, we show him this dialog.
    /// </summary>
    public async Task ShowLoginDialogAsync()
    {
        await _dialogService.ShowAsync<LoginRequiredDialog>(string.Empty, Hardcoded.DialogOptions);
    }
}