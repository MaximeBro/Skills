using Microsoft.AspNetCore.Components;
using MudBlazor;
using Skills.Components.Components;
using Skills.Models;

namespace Skills.Components.Pages.UsersPages.Overview;

public partial class UserEducations : FullComponentBase
{
    [Parameter] public UserModel User { get; set; } = null!;
    
    private List<BreadcrumbItem> _breadcrumbs = [];

    protected override async Task OnInitializedAsync()
    {
        _breadcrumbs.Add(new BreadcrumbItem("Accueil", "/"));
        _breadcrumbs.Add(new BreadcrumbItem("Utilisateurs", "/users"));
        _breadcrumbs.Add(new BreadcrumbItem(User.Name, $"/overview/{User.Username}"));
        _breadcrumbs.Add(new BreadcrumbItem("Diplômes", null, true));

        await RefreshDataAsync();
    }

    private async Task RefreshDataAsync()
    {
        
    }
}