using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Skills.Services;

namespace Skills.Components.Layout;

public partial class MainLayout
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Inject] public IWebHostEnvironment Environment { get; set; } = null!;
    [Inject] public ADAuthenticationService AuthenticationService { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public ThemeManager ThemeManager { get; set; } = null!; 
    
    private MudTheme _theme = new();
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
        
        _theme.Palette.Primary = ThemeManager.GetMudColor(Color.Primary);
        _theme.Palette.Secondary = ThemeManager.GetMudColor(Color.Secondary);
    }

    public void Toggle()
    {
        _docked = !_docked;
        StateHasChanged();
    }

    // private async Task TryNavigateToAsync(string uri)
    // {
    //     var authorized = await AuthenticationService.IsAuthenticatedAsync(AuthenticationState);
    //     if (!authorized)
    //     {
    //         await AuthenticationService.ShowLoginDialogAsync();
    //         return;
    //     }
    //     
    //     NavManager.NavigateTo(uri, true);
    // }
}