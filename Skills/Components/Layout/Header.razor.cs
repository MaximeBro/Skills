using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Skills.Services;

namespace Skills.Components.Layout;

public partial class Header
{
    [Inject] public AuthenticationService AuthenticationService { get; set; } = null!;

    private bool _docked = false;
    private IIdentity? _identity;

    protected override async Task OnInitializedAsync()
    {
        _identity = await AuthenticationService.GetUserIdentityAsync();
    }

    private void Toggle()
    {
        _docked = true;
        StateHasChanged();
    }
}