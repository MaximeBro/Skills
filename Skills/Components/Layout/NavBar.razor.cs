using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Skills.Components.Components;
using Skills.Services;

namespace Skills.Components.Layout;

public partial class NavBar : FullComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Parameter] public bool IsDarkTheme { get; set; }
    [Inject] public IWebHostEnvironment Environment { get; set; } = null!;
    [Inject] public ADAuthenticationService AuthenticationService { get; set; } = null!;
    
    private bool _docked = false;
    private IIdentity? _identity;
    
    protected override async Task OnInitializedAsync()
    {
        var authenticated = await AuthenticationService.IsAuthenticatedAsync(AuthenticationState);
        if (authenticated)
        {
            var authState = await AuthenticationState;
            _identity = authState.User.Identity;
        }
        
        ThemeManager.OnPaletteChanged += async() => await InvokeAsync(StateHasChanged);
    }

    public void Toggle()
    {
        _docked = !_docked;
        StateHasChanged();
    }
}