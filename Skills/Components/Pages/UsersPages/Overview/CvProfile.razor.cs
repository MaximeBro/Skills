using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using Skills.Components.Dialogs;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Models.CV;
using Skills.Models.Enums;
using Skills.Services;

namespace Skills.Components.Pages.UsersPages.Overview;

public partial class CvProfile : ComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public WordExportService WordExportService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    [Parameter] public UserModel User { get; set; } = null!;
    private List<BreadcrumbItem> _breadcrumbs = new();

    private List<CvInfo> _cvs = new();

    private bool _sortMostRecent = false;
    private string _sortActionText => _sortMostRecent ? "Du plus récent au plus ancien" : "Du plus ancien au plus récent";
    private string _sortActionIcon => _sortMostRecent ? "fas fa-arrow-down-1-9" : "fas fa-arrow-up-9-1";

    private bool _loading;

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
        var cv = new CvInfo
        {
            UserId = User.Id,
            Title = $"CV{(db.CVs.AsNoTracking().Any(x => x.UserId == User.Id) ? $" ({db.CVs.AsNoTracking().Count(x => x.UserId == User.Id)})" : string.Empty)}"
        };
        db.CVs.Add(cv);
        await db.SaveChangesAsync();
        await db.DisposeAsync();
        
        NavManager.NavigateTo($"/overview/{User.Username}/cv-editor/{cv.Id}");
    }
    
    private async Task DeleteCvAsync(CvInfo cv)
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, $"Voulez-vous vraiment supprimer ce CV ? \nCette action est irréversible !" } };
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

    private async Task ExportCvAsync(CvInfo cv)
    {
        _loading = true;
        StateHasChanged();
        var transaction = await WordExportService.ExportCvAsync<MemoryStream>(cv);
        if (transaction.State == ImportState.Successful)
        {
            var streamRef = new DotNetStreamReference(transaction.Value!);
            await JsRuntime.InvokeVoidAsync("downloadFileFromStream", $"CV_{User.Username}.docx", streamRef);
            Snackbar.Add($"CV de {User.Name} exporté avec succès !", Severity.Success);
        }
        else if (transaction.State == ImportState.Skipped)
        {
            Snackbar.Add($"{transaction.Message}", Severity.Warning);
        }
        else if(transaction.State == ImportState.Crashed)
        {
            Snackbar.Add($"{transaction.Message}", Severity.Error);
            Snackbar.Add($"{transaction.ErrorMessage}", Severity.Error);
        }

        _loading = false;
        StateHasChanged();
    }

    private void OnSortChanged()
    {
        _sortMostRecent = !_sortMostRecent;
        if (_sortMostRecent) _cvs = _cvs.OrderBy(x => x.CreatedAt).ToList();
        else _cvs = _cvs.OrderByDescending(x => x.CreatedAt).ToList();
        StateHasChanged();
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _cvs = db.CVs.AsNoTracking()
                     .Include(x => x.Skills).ThenInclude(x => x.Skill)
                     .Include(x => x.Education).Include(x => x.Experiences)
                     .Include(x => x.Certifications).Include(x => x.SafetyCertifications).ThenInclude(x => x.Certification)
                     .OrderByDescending(x => x.CreatedAt)
                     .ToList();

        await db.DisposeAsync();
    }
}