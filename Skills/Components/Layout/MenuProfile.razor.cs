using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Skills.Components.Layout;

public partial class MenuProfile : AuthorizedComponent
{
    private ClaimsPrincipal _user = null!;
    private string _name = string.Empty;
    private string _email = string.Empty;

    private MudMenu _menu;
    
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationState;
        _user = authState.User;
        _name = _user.FindFirstValue("name") ?? "Undefined";
        _email = _user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
    }
}