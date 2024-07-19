using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using Skills.Components.Components;
using Skills.Components.Dialogs;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Models.Enums;
using Skills.Services;

namespace Skills.Components.Pages.Admin.SkillsTabs;

public partial class SkillsMapping : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public SkillService SkillService { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    [Parameter] public SkillsManagement Manager { get; set; } = null!;
    [Parameter] public string Title { get; set; } = string.Empty;

    private Dictionary<Guid, List<TypeLevel>> _skillTypeLevels = new();
    private Dictionary<Guid, List<SoftTypeLevel>> _softSkillTypeLevels = new();
    private List<AbstractSkillModel> _models = new();
    private string _search = string.Empty;
    private bool _loading;

    private MudDataGrid<AbstractSkillModel> _grid = null!;

    private Func<AbstractSkillModel, bool> QuickFilter => x =>
    {
        if (!string.IsNullOrWhiteSpace(x.Type) && x.Type.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (!string.IsNullOrWhiteSpace(x.Category) && x.Category.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (!string.IsNullOrWhiteSpace(x.SubCategory) && x.SubCategory.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (!string.IsNullOrWhiteSpace(x.Description) && x.Description.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;

        return false;
    };

    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await _grid.ExtendSortAsync(nameof(AbstractSkillModel.Category), SortDirection.Ascending, x => x.Category, new MudBlazor.Utilities.NaturalComparer());
        }
    }

    private async Task CreateSkillAsync()
    {
        var instance = await DialogService.ShowAsync<SkillDialog>(string.Empty, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: SkillModel model })
        {
            var db = await Factory.CreateDbContextAsync();
            db.Skills.Add(model);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }

    private async Task CreateSoftSkillAsync()
    {
        var instance = await DialogService.ShowAsync<SoftSkillDialog>(string.Empty, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: SoftSkillEditModel model })
        {
            var db = await Factory.CreateDbContextAsync();
            var type = db.SkillsTypes.AsNoTracking().First(x => x.Type == SkillDataType.Type && x.Value == Hardcoded.SoftSkill);
            var softSkill = new SoftSkillModel { Type = type.Value, TypeId = type.Id, Description = model.SoftSkill.Description };
            db.SoftSkills.Add(softSkill);
            foreach (var level in model.Levels) level.SkillId = softSkill.Id;
            db.SoftTypesLevels.AddRange(model.Levels);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }

    private async Task EditSkillAsync(AbstractSkillModel model)
    {
        var parameters = new DialogParameters<SkillDialog> { { x => x.Model, model } };
        var instance = await DialogService.ShowAsync<SkillDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: SkillModel skillModel })
        {
            var db = await Factory.CreateDbContextAsync();

            var oldModel = db.Skills.AsTracking().FirstOrDefault(x => x.Id == model.Id);
            if (oldModel != null)
            {
                oldModel.TypeId = skillModel.TypeId;
                oldModel.CategoryId = skillModel.CategoryId;
                oldModel.SubCategoryId = skillModel.SubCategoryId;
                oldModel.Description = skillModel.Description;
                db.Skills.Update(oldModel);
                await db.SaveChangesAsync();
            }
            else
            {
                Snackbar.Add(
                    "La compétence est introuvable ! Elle a peut être été modifiée ou supprimée par un autre utilisateur entre temps.",
                    Severity.Error);
                return;
            }

            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }

    private async Task EditSoftSkillAsync(AbstractSkillModel model)
    {
        var db = await Factory.CreateDbContextAsync();
        var editModel = new SoftSkillEditModel
        {
            SoftSkill = model,
            Levels = db.SoftTypesLevels.Where(x => x.SkillId == model.Id).ToList()
        };
        await db.DisposeAsync();
        
        var parameters = new DialogParameters<SoftSkillDialog> { { x => x.Model, editModel } };
        var instance = await DialogService.ShowAsync<SoftSkillDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: SoftSkillEditModel skillEditModel })
        {
            db = await Factory.CreateDbContextAsync();

            var oldModel = db.SoftSkills.AsTracking().FirstOrDefault(x => x.Id == model.Id);
            if (oldModel != null)
            {
                oldModel.Description = skillEditModel.SoftSkill.Description;
                db.SoftSkills.Update(oldModel);
                foreach (var level in skillEditModel.Levels)
                {
                    var oldLevel = db.SoftTypesLevels.FirstOrDefault(x => x.SkillId == model.Id && x.Level == level.Level);
                    if (oldLevel != null)
                    {
                        oldLevel.Value = level.Value;
                        db.SoftTypesLevels.Update(oldLevel);
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        level.SkillId = oldModel.Id;
                        db.SoftTypesLevels.Add(level);
                        await db.SaveChangesAsync();
                    }
                }
                
                await db.SaveChangesAsync();
            }
            else
            {
                Snackbar.Add(
                    "La compétence est introuvable ! Elle a peut être été modifiée ou supprimée par un autre utilisateur entre temps.",
                    Severity.Error);
                return;
            }

            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }

    private async Task DeleteSkillAsync(AbstractSkillModel model)
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, "Voulez-vous vraiment supprimer cette compétence ? Cette action est irréversible !" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            var db = await Factory.CreateDbContextAsync();
            if (model is SoftSkillModel)
            {
                var old = db.SoftSkills.FirstOrDefault(x => x.Id == model.Id);
                if (old != null) db.SoftSkills.Remove(old);
            }
            else
            {
                var old = db.Skills.FirstOrDefault(x => x.Id == model.Id);
                if (old != null) db.Skills.Remove(old);
            }

            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }

    private async Task PurgeSkillsAsync()
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, $"Voulez-vous vraiment supprimer toutes les compétences et tous les types de la base ? \nCette action est irréversible !" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            _loading = true;
            StateHasChanged();
            await SkillService.PurgeAllAsync();
            await RefreshDataAsync();
            await Manager.RefreshSkillsTypesAsync();
            _loading = false;
            StateHasChanged();
        }
    }

    private async Task ImportSkillsAsync()
    {
        var instance = await DialogService.ShowAsync<ImportSkillsDialog>(string.Empty, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: IBrowserFile file })
        {
            var stream = file.OpenReadStream();
            _loading = true;
            StateHasChanged();
            var response = await SkillService.ImportXlsxAsync(stream, "A1");
            switch (response.Key)
            {
                case ImportState.Cancelled:
                {
                    _loading = false;
                    Snackbar.Add(response.Value, Severity.Warning);
                    break;
                }
                
                case ImportState.Crashed:
                {
                    _loading = false;
                    Snackbar.Add(response.Value, Severity.Error);
                    break;
                }
                
                case ImportState.Skipped:
                {
                    _loading = false;
                    Snackbar.Add(response.Value, Severity.Info);
                    break;
                }
                
                case ImportState.Successful:
                {
                    _loading = false;
                    await RefreshDataAsync();
                    Snackbar.Add(response.Value, Severity.Success);
                    await Manager.RefreshSkillsTypesAsync();
                    break;
                }
            }
            StateHasChanged();
        }
    }

    private async Task ExportSkillsAsync()
    {
        var toExport = _models.Where(QuickFilter).ToList();
        var levels = _skillTypeLevels.Where(x => toExport.Select(y => y.Id).Contains(x.Key)).ToDictionary();
        var softLevels = _softSkillTypeLevels.Where(x => toExport.Select(y => y.Id).Contains(x.Key)).ToDictionary();

        var stream = await SkillService.ExportSkillsAsync(toExport, levels, softLevels);
        using var streamRef = new DotNetStreamReference(stream);
        await JsRuntime.InvokeVoidAsync("downloadFileFromStream", $"Compétences-{DateTime.Now:dd-MM-yyyy}.xlsx", streamRef);
    }

    private async Task OnRowClickedAsync(DataGridRowClickEventArgs<AbstractSkillModel> args)
    {
        if (args.MouseEventArgs.Detail == 2)
        {
            if (args.Item.Type == Hardcoded.SoftSkill)
            {
                await EditSoftSkillAsync(args.Item);
            }
            else
            {
                await EditSkillAsync(args.Item);
            }
        }
    }

    private async Task EditModelAsync(AbstractSkillModel model)
    {
        if (model.Type == Hardcoded.SoftSkill)
        {
            await EditSoftSkillAsync(model);
        }
        else
        {
            await EditSkillAsync(model);
        }
    }
    
    public override async Task RefreshDataAsync()
    {
        _loading = true;
        var db = await Factory.CreateDbContextAsync();
        var skillModels = await db.Skills.AsNoTracking()
            .Include(x => x.TypeInfo) // Includes are used for edition, don't delete them !
            .Include(x => x.CategoryInfo)
            .Include(x => x.SubCategoryInfo)
            .ToListAsync();

        var softSkillsModels = await db.SoftSkills.AsNoTracking().ToListAsync();

        _models.Clear();
        _models.AddRange(new List<AbstractSkillModel>(skillModels));
        _models.AddRange(new List<AbstractSkillModel>(softSkillsModels));

        _skillTypeLevels.Clear();
        _softSkillTypeLevels.Clear();
        foreach (var model in _models) _skillTypeLevels.Add(model.Id, db.TypesLevels.AsNoTracking().Where(x => x.TypeId == model.TypeId).ToList());
        foreach (var model in _models) _softSkillTypeLevels.Add(model.Id, db.SoftTypesLevels.AsNoTracking().Where(x => x.SkillId == model.Id).ToList());
        
        await db.DisposeAsync();
        _loading = false;
    }
}