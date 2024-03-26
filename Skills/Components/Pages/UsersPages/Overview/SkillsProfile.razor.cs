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
                model.UserId = User.Id;
                db.Userskills.Add(model);
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

    private async Task ReviewSkillAsync(UserSkillModel userSkillModel)
    {
        
    }
    
    private async Task RevokeSkillAsync(Guid skillId)
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, $"Voulez-vous vraiment retirer cette compétence à {User.Name} ? Cette action est irréversible !" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            var db = await Factory.CreateDbContextAsync();
            var userSkill = await db.Userskills.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == User.Id && x.SkillId == skillId);
            if (userSkill != null)
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
        _userSkillsModels = await db.Userskills.AsNoTracking().ToListAsync();
        _userSkills = await db.Skills.AsNoTracking()
                                     .Where(x => _userSkillsModels.Select(x => x.SkillId).Contains(x.Id))
                                     .Include(x => x.Type)
                                     .Include(x => x.Category)
                                     .Include(x => x.SubCategory)
                                     .ToListAsync();
    }
}