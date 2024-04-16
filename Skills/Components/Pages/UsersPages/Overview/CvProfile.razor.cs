using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Dialogs;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Models.CV;

namespace Skills.Components.Pages.UsersPages.Overview;

public partial class CvProfile : ComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Parameter] public UserModel User { get; set; } = null!;
    private List<BreadcrumbItem> _breadcrumbs = new();

    private List<CvInfo> _cvs = new();

    protected override async Task OnInitializedAsync()
    {
        _breadcrumbs.Add(new BreadcrumbItem("Accueil", "/"));
        _breadcrumbs.Add(new BreadcrumbItem("Utilisateurs", "/users"));
        _breadcrumbs.Add(new BreadcrumbItem(User.Name, $"/overview/{User.Username}"));
        _breadcrumbs.Add(new BreadcrumbItem("CV", null, true));

        await RefreshDataAsync();
    }

    private async Task CreateCvAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        db.CVs.Add(new CvInfo
        {
            UserId = User.Id,
            Title = $"CV{(db.CVs.AsNoTracking().Any(x => x.UserId == User.Id) ? $" ({db.CVs.AsNoTracking().Count(x => x.UserId == User.Id)})" : string.Empty)}"
        });
        await db.SaveChangesAsync();
        await db.DisposeAsync();
        
        NavManager.NavigateTo("/");
    }
    
    private async Task DeleteCvAsync(CvInfo cv)
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, $"Voulez-vous vraiment supprimer ce CV ? Cette action est irréversible !" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            var db = await Factory.CreateDbContextAsync();
            var oldCv = await db.CVs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == cv.Id);
            if (oldCv != null)
            {
                db.CVs.Remove(oldCv);
                await db.SaveChangesAsync();
            }
            await db.DisposeAsync();
            await RefreshDataAsync(); 
        }
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _cvs = db.CVs.AsNoTracking()
                     .Include(x => x.Skills).Include(x => x.SoftSkills)
                     .Include(x => x.Education).Include(x => x.Experiences)
                     .Include(x => x.Certifications).Include(x => x.SafetyCertifications)
                     .ToList();

        await db.DisposeAsync();
    }
}