using System.Security.Claims;
namespace Skills.Components.Layout;

public partial class MenuProfile : AuthorizedComponent
{
    private ClaimsPrincipal _user = null!;
    private string _name = string.Empty;
    private string _email = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationState;
        _user = authState.User;
        _name = _user.FindFirstValue("name") ?? "Undefined";
        _email = _user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
    }
}