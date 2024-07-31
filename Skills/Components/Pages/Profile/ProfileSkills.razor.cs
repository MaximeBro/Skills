using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
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

namespace Skills.Components.Pages.Profile;

public partial class ProfileSkills : FullComponentBase
{
    [Parameter] public UserModel User { get; set; } = null!;
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] public SkillService SkillService { get; set; } = null!;
    [Inject] public IconHelperService IconHelperService { get; set; } = null!;

    private List<BreadcrumbItem> _breadcrumbs = new();
    private List<UserSkillModel> _userSkillsModels = new();
    private List<AbstractSkillModel> _userSkills = new();
    private Dictionary<Guid, CommonTypeModel?> _selectedLevels = new();
    private Dictionary<Guid, List<CommonTypeModel>> _rowSkillLevels = new();

    private string _search = string.Empty;

    private Func<UserSkillModel, bool> QuickFilter => x =>
    {
        if (x.Skill != null)
        {
            if (!string.IsNullOrWhiteSpace(x.Skill.Type) && x.Skill.Type.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
            if (!string.IsNullOrWhiteSpace(x.Skill.Category) && x.Skill.Category.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
            if (!string.IsNullOrWhiteSpace(x.Skill.SubCategory) && x.Skill.SubCategory.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
            if (!string.IsNullOrWhiteSpace(x.Skill.Description) && x.Skill.Description.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        } 
        
        return false;
    };
    
    protected override async Task OnInitializedAsync()
    {
        _breadcrumbs.Add(new BreadcrumbItem("Accueil", "/"));
        _breadcrumbs.Add(new BreadcrumbItem("Utilisateurs", "/users"));
        _breadcrumbs.Add(new BreadcrumbItem(User.Name, $"/overview/{User.Username}"));
        _breadcrumbs.Add(new BreadcrumbItem("Compétences", null, true));
        
        await RefreshDataAsync();
        StateHasChanged();
    }
    

    private async Task ManageSkillsAsync()
    {
        var parameters = new DialogParameters<ManageSkillsDialog> { { x => x.User, User } };
        var options = Hardcoded.DialogOptions;
        options.MaxWidth = MaxWidth.ExtraLarge;
        var instance = await DialogService.ShowAsync<ManageSkillsDialog>(string.Empty, parameters, options);
        var result = await instance.Result;
        if (result is { Data: IEnumerable<AbstractSkillModel> skills })
        {
            var db = await Factory.CreateDbContextAsync();
            var ownedSkills = db.UsersSkills.Where(x => x.UserId == User.Id).ToList(); // Current assigned skills, before the update
            var oldOwnedSkills = ownedSkills.Where(x => !skills.Select(y => y.Id).Contains(x.SkillId)).ToList(); 
            var newOwnedSkills = skills.Where(x => !ownedSkills.Select(y => y.SkillId).Contains(x.Id)).ToList();
            
            db.UsersSkills.RemoveRange(oldOwnedSkills); // Removes old skill that are no longer assigned to the user
            db.UsersSkills.AddRange(newOwnedSkills.Select(x => new UserSkillModel { UserId = User.Id, SkillId = x.Id, Level = 0 })); // Adds new skills to the user
            
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await RefreshDataAsync();
            StateHasChanged();

            await SendUpdateAsync();
        }
    }

    private async Task LevelChangedAsync(int level, UserSkillModel model)
    {
        var db = await Factory.CreateDbContextAsync();
        if (db.UsersSkills.Any(x => x.UserId == User.Id && x.SkillId == model.SkillId))
        {
            model.Level = level;
            db.UsersSkills.Update(model);
            await db.SaveChangesAsync();
            Snackbar.Add("Modification enregistrée !", Severity.Success, options =>
            {
                options.VisibleStateDuration = 1000;
                options.ShowCloseIcon = false;
                options.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
            });
        }
        
        await db.DisposeAsync();
        await RefreshDataAsync();
        StateHasChanged();

        await SendUpdateAsync();
    }
    
    private async Task RevokeSkillAsync(UserSkillModel model)
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, $"Voulez-vous vraiment retirer cette compétence à {User.Name} ? Cette action est irréversible !" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result.Data != null && (bool)result.Data)
        {
            var db = await Factory.CreateDbContextAsync();
            var userSkill = await db.UsersSkills.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == model.UserId && x.SkillId == model.SkillId);
            if ( userSkill != null)
            {
                db.UsersSkills.Remove(userSkill);
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
            StateHasChanged();

            await SendUpdateAsync();
        }
    }

    private async Task ExportSkillsAsync()
    {
        var db = await Factory.CreateDbContextAsync();

        List<AbstractSkillModel> skills = new();
        skills.AddRange(db.Skills.AsNoTracking().ToList());
        skills.AddRange(db.SoftSkills.AsNoTracking().ToList());

        var data = new Dictionary<AbstractSkillModel, int>();
        foreach (var skill in skills)
        {
            int level = 0;
            var ownedSkill = _userSkillsModels.FirstOrDefault(x => x.SkillId == skill.Id);
            if (ownedSkill != null)
            {
                level = ownedSkill.Level;
            }
            
            data.Add(skill, level);
        }
        await db.DisposeAsync();
        
        var stream = await SkillService.ExportUserSkillsAsync(data, User);
        using var streamRef = new DotNetStreamReference(stream);
        await JsRuntime.InvokeVoidAsync("downloadFileFromStream", $"Compétences_{User.Username}.xlsx", streamRef);
    }

    private async Task ImportSkillsAsync()
    {
        var instance = await DialogService.ShowAsync<ImportSkillsDialog>(string.Empty, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: IBrowserFile file })
        {
            var stream = file.OpenReadStream();
            var response = await SkillService.ImportUserSkillAsync(stream, "A2", User);
            switch (response.Key)
            {
                case ImportState.Cancelled:
                {
                    Snackbar.Add(response.Value, Severity.Warning);
                    break;
                }
                
                case ImportState.Crashed:
                {
                    Snackbar.Add(response.Value, Severity.Error);
                    break;
                }
                
                case ImportState.Skipped:
                {
                    Snackbar.Add(response.Value, Severity.Info);
                    break;
                }
                
                case ImportState.Successful:
                {
                    await RefreshDataAsync();
                    StateHasChanged();
                    
                    Snackbar.Add(response.Value, Severity.Success);
                    await SendUpdateAsync();
                    break;
                }
            }
        }
    }
    
    public override async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _userSkillsModels = await db.UsersSkills.AsNoTracking()
                                               .Where(x => x.UserId == User.Id)
                                               .Include(x => x.Skill).ThenInclude(x => x!.TypeInfo)
                                               .Include(x => x.User).ThenInclude(x => x!.Group)
                                               .ToListAsync();
        
        var skillModels = await db.Skills.AsNoTracking()
                                                       .Where(x => _userSkillsModels.Select(y => y.SkillId).Contains(x.Id))
                                                       .Include(x => x.TypeInfo)
                                                       .Include(x => x.CategoryInfo)
                                                       .Include(x => x.SubCategoryInfo)
                                                       .ToListAsync();

        var softSkillsModels = await db.SoftSkills.AsNoTracking()
                                                                   .Where(x => _userSkillsModels.Select(y => y.SkillId).Contains(x.Id))
                                                                   .Include(x => x.TypeInfo)
                                                                   .ToListAsync();
        
        foreach (var model in softSkillsModels) model.Type = model.TypeInfo.Value;
        foreach (var model in skillModels)
        {
            model.Type = model.TypeInfo.Value;
            model.Category = model.CategoryInfo.Value;
            model.SubCategory = model.SubCategoryInfo?.Value ?? string.Empty;
        }
        
        _selectedLevels.Clear();
        _rowSkillLevels.Clear();
        foreach(var skill in _userSkillsModels)
        {
            var level = db.TypesLevels.AsNoTracking().FirstOrDefault(x => x.TypeId == skill.Skill!.TypeId && x.Level == skill.Level)?.ToAbstract() ??
                                          db.SoftTypesLevels.AsNoTracking().FirstOrDefault(x => x.SkillId == skill.SkillId && x.Level == skill.Level)?.ToAbstract();
            _selectedLevels.Add(skill.SkillId, level); // The level has to be not null !
            
            var skillLevels = skill.Skill!.Type!.Equals(Hardcoded.SoftSkill, StringComparison.CurrentCultureIgnoreCase) ? // If it is a soft skill ... else ...
                                                            db.SoftTypesLevels.Where(x => x.SkillId == skill.SkillId).Select(x => x.ToAbstract()) : 
                                                            db.TypesLevels.Where(x => x.TypeId == skill.Skill!.TypeId).Select(x => x.ToAbstract());
            _rowSkillLevels.Add(skill.SkillId, skillLevels.ToList());
        }
        
        _userSkills.Clear();
        _userSkills.AddRange(new List<AbstractSkillModel>(skillModels));
        _userSkills.AddRange(new List<AbstractSkillModel>(softSkillsModels));

        foreach (var userSkill in _userSkillsModels) userSkill.Skill = _userSkills.FirstOrDefault(x => x.Id == userSkill.SkillId);
    }
}