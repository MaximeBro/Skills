using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.Graph.ExternalConnectors;
using Skills.Services;

namespace Skills.Components.Layout;

public partial class MenuProfile : ComponentBase
{
    [Inject] public AuthenticationService AuthenticationService { get; set; } = null!;

    private ClaimsPrincipal _user = null!;
    private string _name = string.Empty;
    private string _email = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        _user = await AuthenticationService.GetUserAsync();
        _name = _user.FindFirstValue("name") ?? string.Empty;
        _email = _user.Identity?.Name ?? string.Empty;
    }
}