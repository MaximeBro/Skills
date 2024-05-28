using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Models;

namespace Skills.Components.Pages.UsersPages.Overview;

public partial class ProfileInfos : FullComponentBase
{
    [CascadingParameter(Name = nameof(UsersOverview))] public UsersOverview Overview { get; set; } = null!;
    [Parameter] public UserModel User { get; set; } = null!;
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;

    private List<BreadcrumbItem> _breadcrumbs = [];

    private string _phoneNumber = string.Empty;
    private string _job = string.Empty;
    private DateTime? _birthDate;
    
    protected override async Task OnInitializedAsync()
    {
        _breadcrumbs.Add(new BreadcrumbItem("Accueil", "/"));
        _breadcrumbs.Add(new BreadcrumbItem("Utilisateurs", "/users"));
        _breadcrumbs.Add(new BreadcrumbItem(User.Name, $"/overview/{User.Username}"));
        _breadcrumbs.Add(new BreadcrumbItem("Informations générales", null, true));

        _job = User.Job;
        _phoneNumber = User.PhoneNumber;
        _birthDate = User.BirthDate;
        
        await RefreshDataAsync();
    }
    
    private async Task SaveAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        User.Job = _job;
        User.PhoneNumber = _phoneNumber;
        User.BirthDate = _birthDate;
        db.Update(User);
        await db.SaveChangesAsync();
        await db.DisposeAsync();
        
        await Overview.UserChangedAsync();
        await SendUpdateAsync();
    }

    protected override Task RefreshDataAsync()
    {
        _job = User.Job;
        _phoneNumber = User.PhoneNumber;
        _birthDate = User.BirthDate;

        return Task.CompletedTask;
    }
}