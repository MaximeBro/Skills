using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Dialogs;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;

namespace Skills.Components.Pages.UsersPages.Overview;

public partial class SkillsProfile : ComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Parameter] public UserModel User { get; set; } = null!;

    private List<UserSkillModel> _userSkillsModels = new();
    private List<SkillModel> _userSkills = new();
    private Dictionary<Guid, string> _selectedLevels = new();

    private string _search = string.Empty;

    private Func<UserSkillModel, bool> QuickFilter => x =>
    {
        if (x.Skill != null)
        {
            if (x.Skill.Type.Value.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
            if (x.Skill.Category.Value.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
            if (x.Skill.SubCategory != null && x.Skill.SubCategory.Value.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
            if (x.Skill.Description != null && x.Skill.Description.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        } 
        
        return false;
    };
    
    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
    }

    private async Task GrantSkillAsync()
    {
        var instance = await DialogService.ShowAsync<UserSkillDialog>(string.Empty, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: UserSkillModel model })
        {
            await RefreshDataAsync();
            if (_userSkillsModels.FirstOrDefault(x => x.SkillId == model.SkillId) is null)
            {
                var db = await Factory.CreateDbContextAsync();
                var newModel = new UserSkillModel
                {
                    UserId = User.Id,
                    SkillId = model.SkillId,
                    Level = model.Level
                };

                var user = db.Users.FirstOrDefault(x => x.Id == User.Id);

                db.Userskills.Add(newModel);
                await db.SaveChangesAsync();
                await db.DisposeAsync();
                await RefreshDataAsync();
            }
            else
            {
                Snackbar.Add("Cet utilisateur possède déjà cette compétence !", Severity.Error);
                return;
            }
        }
    }

    private async Task ManageSkillAsync(UserSkillModel userSkill)
    {
        var parameters = new DialogParameters<UserSkillDialog> { { x => x.UserSkill, userSkill } };
        var instance = await DialogService.ShowAsync<UserSkillDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: UserSkillModel model })
        {
            var db = await Factory.CreateDbContextAsync();
            db.Userskills.Remove(userSkill);
            db.Userskills.Add(new UserSkillModel
            {
                UserId = User.Id,
                SkillId = model.SkillId,
                Level = model.Level
            });
            
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
        }
    }
    
    private async Task RevokeSkillAsync(UserSkillModel model)
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, $"Voulez-vous vraiment retirer cette compétence à {User.Name} ? Cette action est irréversible !" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            var db = await Factory.CreateDbContextAsync();
            var userSkill = await db.Userskills.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == model.UserId && x.SkillId == model.SkillId);
            if ( userSkill != null)
            {
                db.Userskills.Remove(userSkill);
                await db.SaveChangesAsync();
                await db.DisposeAsync();
            }
            else
            {
                await db.DisposeAsync();
                Snackbar.Add($"Compétence introuvable pour l'utilisateur {User.Name} ! Une autre modification a peut être eu lieu au même moment.", Severity.Error);
                return;
            }
            
            await RefreshDataAsync();
        }
    }
    
    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _userSkillsModels = await db.Userskills.AsNoTracking()
                                                .Where(x => x.UserId == User.Id)
                                               .Include(x => x.Skill).ThenInclude(x => x!.Type)
                                               .Include(x => x.Skill).ThenInclude(x => x!.Category)
                                               .Include(x => x.Skill).ThenInclude(x => x!.SubCategory)
                                               .Include(x => x.User).ThenInclude(x => x!.Group)
                                               .ToListAsync();
        
        _userSkills = await db.Skills.AsNoTracking()
                                     .Where(x => _userSkillsModels.Select(y => y.SkillId).Contains(x.Id))
                                     .Include(x => x.Type)
                                     .Include(x => x.Category)
                                     .Include(x => x.SubCategory)
                                     .ToListAsync();
        
        _selectedLevels.Clear();
        foreach(var skill in _userSkillsModels) _selectedLevels.Add(skill.SkillId, db.TypesLevels.AsNoTracking().FirstOrDefault(x => x.TypeId == skill.Skill!.TypeId && x.Level == skill.Level)?.Value ?? string.Empty);
    }
}