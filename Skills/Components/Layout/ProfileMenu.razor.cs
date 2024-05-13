using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Skills.Components.Components;

namespace Skills.Components.Layout;

public partial class ProfileMenu : FullComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Parameter] public string Class { get; set; } = string.Empty;
    
    private ClaimsPrincipal _user = null!;
    private string _name = string.Empty;
    private string _email = string.Empty;

    private MudMenu _menu = null!;
    
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationState;
        _user = authState.User;
        _name = _user.FindFirstValue("name") ?? string.Empty;
        _email = _user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
    }

    private void ToOverview() => NavManager.NavigateTo($"/overview/{_email.Replace("@sasp.fr", string.Empty)}", true);   
    private void ToLogin() => NavManager.NavigateTo("/connexion", true);   
}