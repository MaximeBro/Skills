using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Components.Dialogs;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Models.Enums;

namespace Skills.Components.Pages.Admin.SkillsTabs;

public partial class SkillsTypes : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Parameter] public SkillsManagement Manager { get; set; } = null!;
    [Parameter] public string Title { get; set; } = string.Empty;

    private Dictionary<Guid, bool> _toggledPanes = new();
    
    private List<SKillInfo> _types = new();
    private List<SKillInfo> _categories = new();
    private List<SKillInfo> _subcategories = new();

    private SingleStringModel _typeModel = new();
    private SingleStringModel _categoryModel = new();
    private SingleStringModel _subCategoryModel = new();

    private MudTextField<string> _typeInput = null!;
    private MudTextField<string> _categoryInput = null!;
    private MudTextField<string> _subCategoryInput = null!;

    private string _levelZeroText = string.Empty;
    private string _levelOneText = string.Empty;
    private string _levelTwoText = string.Empty;
    private string _levelThreeText = string.Empty;
    private string _levelFourText = string.Empty;

    private bool _loading;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await RefreshPageDataAsync();
    }

    public void SetSaving(bool saving)
    {
        _loading = saving;
        StateHasChanged();
    }

    private async Task AddAsync(SkillDataType type)
    {
        SKillInfo skillInfo = new SKillInfo { Type = type };
        switch (type)
        {
            case SkillDataType.Type:
            {
                if (string.IsNullOrEmpty(_typeModel.Value)) return;
                skillInfo.Value = _typeModel.Value;
                await _typeInput.Clear();
                _toggledPanes.Add(skillInfo.Id, false);
                break;
            }
            case SkillDataType.Category:
            {
                if (string.IsNullOrEmpty(_categoryModel.Value)) return;
                skillInfo.Value = _categoryModel.Value;
                await _categoryInput.Clear();
                break;
            }
            case SkillDataType.SubCategory:
            {
                if (string.IsNullOrEmpty(_subCategoryModel.Value)) return;
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
            _toggledPanes.Remove(model.Id);
            var db = await Factory.CreateDbContextAsync();
            db.SkillsTypes.Remove(model);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
            await Manager.RefreshSkillsAsync();
        }
    }

    public async Task RefreshSkillsAsync()
    {
        await Manager.RefreshSkillsAsync();
    }
    
    public async Task RefreshPageDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _types = db.SkillsTypes.AsNoTracking().Where(x => x.Type == SkillDataType.Type && x.Value.ToUpper() != "SOFT-SKILL").ToList(); // It is very important to prevent anybody from editing the Soft-Skill Type or it will break a lot of stuff !
        _categories = db.SkillsTypes.AsNoTracking().Where(x => x.Type == SkillDataType.Category).ToList();
        _subcategories = db.SkillsTypes.AsNoTracking().Where(x => x.Type == SkillDataType.SubCategory).ToList();
        await db.DisposeAsync();
        
        _toggledPanes.Clear();
        foreach(var type in _types) _toggledPanes.Add(type.Id, false);
    }
}