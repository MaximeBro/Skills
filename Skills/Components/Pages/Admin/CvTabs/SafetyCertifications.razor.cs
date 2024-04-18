using System.Security;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Components.Dialogs;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models.CV;

namespace Skills.Components.Pages.Admin.CvTabs;

public partial class SafetyCertifications : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;

    [Parameter] public string Title { get; set; } = null!;

    private List<SafetyCertification> _certifications = new();

    private string _search = string.Empty;

    private Func<SafetyCertification, bool> QuickFilter => x =>
    {
        if (x.Name.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Category.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Enabled.ToString().Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    };

    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
    }

    private async Task CommitChangesAsync(SafetyCertification certification)
    {
        var db = await Factory.CreateDbContextAsync();
        var old = db.SafetyCertifications.FirstOrDefault(x => x.Id == certification.Id);
        if (old != null)
        {
            old.Category = certification.Category;
            old.Name = certification.Name;
            old.Enabled = certification.Enabled;
            db.SafetyCertifications.Update(old);
            await db.SaveChangesAsync();
        }

        await db.DisposeAsync();
        await RefreshDataAsync();
    }

    private async Task ValueChangedAsync(bool value, SafetyCertification certification)
    {
        var db = await Factory.CreateDbContextAsync();
        var old = db.SafetyCertifications.FirstOrDefault(x => x.Id == certification.Id);
        if (old != null)
        {
            old.Enabled = value;
            db.SafetyCertifications.Update(old);
            await db.SaveChangesAsync();
        }

        await db.DisposeAsync();
        await RefreshDataAsync();
    }

    private async Task CreateCertificationAsync()
    {
        var instance = await DialogService.ShowAsync<SafetyCertificationDialog>(string.Empty, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: SafetyCertification newCertification })
        {
            var db = await Factory.CreateDbContextAsync();
            db.SafetyCertifications.Add(newCertification);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }

    private async Task EditCertificationAsync(SafetyCertification certification)
    {
        var parameters = new DialogParameters<SafetyCertificationDialog> { { x => x.Certification, certification } };
        var instance = await DialogService.ShowAsync<SafetyCertificationDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: SafetyCertification newCertification })
        {
            var db = await Factory.CreateDbContextAsync();
            var old = db.SafetyCertifications.AsNoTracking().FirstOrDefault(x => x.Id == newCertification.Id);
            if (old != null)
            {
                db.SafetyCertifications.Update(newCertification);
                await db.SaveChangesAsync();
            }
            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }

    private async Task DeleteCertificationAsync(SafetyCertification certification)
    {
        var parameters = new DialogParameters<ConfirmDialog>
        {
            {
                x => x.Text,
                $"Voulez-vous vraiment supprimer la certification {certification.Name} ? Cette action est irréversible !"
            }
        };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            var db = await Factory.CreateDbContextAsync();
            db.SafetyCertifications.Remove(certification);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _certifications = db.SafetyCertifications.AsNoTracking().ToList();
        await db.DisposeAsync();

        StateHasChanged();
    }
}