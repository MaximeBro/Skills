using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Components.Dialogs;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;

namespace Skills.Components.Pages.Admin.SkillsTabs;

public partial class SkillsMapping : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Parameter] public SkillsManagement Manager { get; set; } = null!;
    [Parameter] public string Title { get; set; } = string.Empty;

    private Dictionary<Guid, List<TypeLevel>> _skillTypeLevels = new();
    private Dictionary<Guid, List<SoftTypeLevel>> _softSkillTypeLevels = new();
    private List<AbstractSkillModel> _models = new();
    private string _search = string.Empty;

    public Func<AbstractSkillModel, bool> QuickFilter => x =>
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
    }

    private async Task CreateSkillAsync()
    {
        var instance = await DialogService.ShowAsync<CreateSkillDialog>(string.Empty, Hardcoded.DialogOptions);
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
        var instance = await DialogService.ShowAsync<CreateSoftSkillDialog>(string.Empty, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: SoftSkillModel model })
        {
            var db = await Factory.CreateDbContextAsync();
            db.SoftSkills.Add(model);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }

    private async Task EditSkillAsync(AbstractSkillModel model)
    {
        var parameters = new DialogParameters<EditSkillDialog> { { x => x.Model, model } };
        var instance = await DialogService.ShowAsync<EditSkillDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: SkillModel skillModel })
        {
            var db = await Factory.CreateDbContextAsync();

            var oldModel = db.Skills.AsNoTracking().FirstOrDefault(x => x.Id == model.Id);
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
                Snackbar.Add("La compétence est introuvable ! Elle a peut être été modifiée ou supprimée par un autre utilisateur entre temps.", Severity.Error);
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

    private async Task OnRowClickedAsync(DataGridRowClickEventArgs<AbstractSkillModel> args)
    {
        if (args.MouseEventArgs.Detail == 2)
        {
            await EditSkillAsync(args.Item);
        }
    }

    public async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        var skillModels = await db.Skills.AsNoTracking()
                                                       .Include(x => x.TypeInfo)
                                                       .Include(x => x.CategoryInfo)
                                                       .Include(x => x.SubCategoryInfo)
                                                       .ToListAsync();

        var softSkillsModels = await db.SoftSkills.AsNoTracking()
                                                                   .Include(x => x.TypeInfo)
                                                                   .ToListAsync();
        
        foreach (var model in skillModels)
        {
            model.Type = model.TypeInfo.Value;
            model.Category = model.CategoryInfo.Value;
            model.SubCategory = model.SubCategoryInfo?.Value ?? string.Empty;
        }
        
        foreach (var model in softSkillsModels)
        {
            model.Type = model.TypeInfo.Value;
        }
        
        _models.Clear();
        _models.AddRange(new List<AbstractSkillModel>(skillModels));
        _models.AddRange(new List<AbstractSkillModel>(softSkillsModels));
        
        _skillTypeLevels.Clear();
        _softSkillTypeLevels.Clear();
        foreach(var model in _models) _skillTypeLevels.Add(model.Id, db.TypesLevels.AsNoTracking().Where(x => x.TypeId == model.TypeId).ToList());
        foreach(var model in _models) _softSkillTypeLevels.Add(model.Id, db.SoftTypesLevels.AsNoTracking().Where(x => x.SkillId == model.Id).ToList());
    }
}