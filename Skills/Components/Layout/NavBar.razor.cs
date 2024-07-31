using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Services;

namespace Skills.Components.Layout;

public partial class NavBar : FullComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Parameter] public bool IsDarkTheme { get; set; }
    
    [Inject] public ADAuthenticationService AuthenticationService { get; set; } = null!;
    
    private bool _docked = false;
    private IIdentity? _identity;
    private int _notificationsCount = 0;

    private bool NotificationBadgeVisible => _identity is { IsAuthenticated: true } && _notificationsCount > 0;
    private string NotificationBadgeClass => NotificationBadgeVisible ? string.Empty : "invisible-badge";
    
    protected override async Task OnInitializedAsync()
    {
        var authenticated = await AuthenticationService.IsAuthenticatedAsync(AuthenticationState);
        if (authenticated)
        {
            var authState = await AuthenticationState;
            _identity = authState.User.Identity;
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            IsDarkTheme = Layout.IsDarkMode;
            StateHasChanged();
        }
    }

    public async Task RefreshThemeAsync(bool isDarkMode)
    {
        IsDarkTheme = isDarkMode;
        await InvokeStateHasChangedAsync();
    }
    
    private void Toggle()
    {
        _docked = !_docked;
        StateHasChanged();
    }

    public void SetNotificationsCount(int count)
    {
        _notificationsCount = count;
        StateHasChanged();
    }
}