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
    private List<SkillModel> _models = new();
    private string _search = string.Empty;

    public Func<SkillModel, bool> QuickFilter => x =>
    {
        if (x.Type.Value.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Category.Value.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.SubCategory != null && x.SubCategory.Value.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
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

    private async Task EditSkillAsync(SkillModel model)
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

    private async Task DeleteSkillAsync(SkillModel model)
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, "Voulez-vous vraiment supprimer cette compétence ? Cette action est irréversible !" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            var db = await Factory.CreateDbContextAsync();
            db.Skills.Remove(model);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }

    private async Task OnRowClickedAsync(DataGridRowClickEventArgs<SkillModel> args)
    {
        if (args.MouseEventArgs.Detail == 2)
        {
            await EditSkillAsync(args.Item);
        }
    }

    public async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _models = await db.Skills.AsNoTracking()
                                 .Include(x => x.Type)
                                 .Include(x => x.Category)
                                 .Include(x => x.SubCategory)
                                 .ToListAsync();
        
        _skillTypeLevels.Clear();
        foreach(var model in _models) _skillTypeLevels.Add(model.Id, db.TypesLevels.AsNoTracking().Where(x => x.TypeId == model.TypeId).ToList());
    }
}