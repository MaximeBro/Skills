using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Dialogs;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Models.Enums;
using Skills.Services;

namespace Skills.Components.Pages.Admin.SkillsTabs;

public partial class SkillsTypes : ComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public ThemeManager ThemeManager { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Parameter] public SkillsManagement Manager { get; set; } = null!;
    [Parameter] public string Title { get; set; } = string.Empty;

    private List<SKillInfo> _types = new();
    private List<SKillInfo> _categories = new();
    private List<SKillInfo> _subcategories = new();

    private SkillTypeModel _typeModel = new();
    private SkillTypeModel _categoryModel = new();
    private SkillTypeModel _subCategoryModel = new();

    private MudTextField<string> _typeInput = null!;
    private MudTextField<string> _categoryInput = null!;
    private MudTextField<string> _subCategoryInput = null!;
    

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await RefreshDataAsync();
    }

    private async Task AddAsync(SkillDataType type)
    {
        SKillInfo skillInfo = new SKillInfo { Type = type };
        switch (type)
        {
            case SkillDataType.Type:
            {
                skillInfo.Value = _typeModel.Value;
                await _typeInput.Clear();
                break;
            }
            case SkillDataType.Category:
            {
                skillInfo.Value = _categoryModel.Value;
                await _categoryInput.Clear();
                break;
            }
            case SkillDataType.SubCategory:
            {
                skillInfo.Value = _subCategoryModel.Value;
                await _subCategoryInput.Clear();
                break;
            }
        }
        
        var db = await Factory.CreateDbContextAsync();
        db.SkillsTypes.Add(skillInfo);
        await db.SaveChangesAsync();
        await db.DisposeAsync();
        await RefreshDataAsync();
    }

    private async Task EditAsync(SKillInfo model)
    {
        var parameters = new DialogParameters<SkillInfoDialog> { { x => x.Type, model.Type }, { x => x.Value, model.Value } };
        var instance = await DialogService.ShowAsync<SkillInfoDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: SKillInfo skillInfo })
        {
            var db = await Factory.CreateDbContextAsync();
            model.Value = skillInfo.Value;
            db.SkillsTypes.Update(model);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
            await Manager.RefreshSkillsAsync();
        } 
    }

    private async Task DeleteAsync(SKillInfo model)
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, $"Voulez-vous vraiment supprimer {model.Type.Humanize()} \"{model.Value}\" ? Cette action est irréversible !" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            var db = await Factory.CreateDbContextAsync();
            db.SkillsTypes.Remove(model);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
            await Manager.RefreshSkillsAsync();
        }
    }
    
    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _types = db.SkillsTypes.AsNoTracking().Where(x => x.Type == SkillDataType.Type).ToList();
        _categories = db.SkillsTypes.AsNoTracking().Where(x => x.Type == SkillDataType.Category).ToList();
        _subcategories = db.SkillsTypes.AsNoTracking().Where(x => x.Type == SkillDataType.SubCategory).ToList();
        await db.DisposeAsync();
    }

    private sealed class SkillTypeModel
    {
        public string Value { get; set; } = string.Empty;
    }
}