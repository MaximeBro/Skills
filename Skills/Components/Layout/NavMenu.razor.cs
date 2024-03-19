using Microsoft.AspNetCore.Components;
using Skills.Services;

namespace Skills.Components.Layout;

public partial class NavMenu : ComponentBase
{
    [CascadingParameter(Name = "HeaderInstance")] public Header Header { get; set; } = null!;
    [Inject] public AuthenticationService AuthenticationService { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;


    private async Task NavigateToAsync(string uri)
    {
        var authorized = await AuthenticationService.HasPermissionAsync();
        if (!authorized)
        {
            await AuthenticationService.ShowLoginDialogAsync();
            return;
        }
        
        NavManager.NavigateTo(uri);
        Header.Toggle(false);
    }
}