using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Databases;
using Skills.Models;

namespace Skills.Components.Components;

public partial class SkillTypeLevels : ComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Parameter] public SKillInfo Type { get; set; } = null!;
    
    private string _levelZeroText = string.Empty;
    private string _levelOneText = string.Empty;
    private string _levelTwoText = string.Empty;
    private string _levelThreeText = string.Empty;
    private string _levelFourText = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
    }

    private async Task SaveAsync(int level)
    {
        var db = await Factory.CreateDbContextAsync();
        var old = db.TypesLevels.AsNoTracking().FirstOrDefault(x => x.TypeId == Type.Id && x.Level == level);
        
        if (old != null)
        {
            old.Level = level;
            old.Value = GetValueOf(level);
            db.TypesLevels.Update(old);
        }
        else
        {
            db.TypesLevels.Add(new TypeLevel
            {
                TypeId = Type.Id,
                Level = level,
                Value = GetValueOf(level)
            });
        }

        Snackbar.Add("Modification enregistrée !", Severity.Success, options =>
        {
            options.VisibleStateDuration = 1000;
            options.ShowCloseIcon = false;
            options.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow;
        });
        await db.SaveChangesAsync();
        await db.DisposeAsync();
    }

    private string GetValueOf(int level)
    {
        return level switch
        {
            0 => _levelZeroText,
            1 => _levelOneText,
            2 => _levelTwoText,
            3 => _levelThreeText,
            4 => _levelFourText,
            _ => string.Empty
        };
    }
    
    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        var levels = db.TypesLevels.AsNoTracking().Where(x => x.TypeId == Type.Id).ToList();
        await db.DisposeAsync();
        
        _levelZeroText = levels.FirstOrDefault(x => x.Level == 0)?.Value ?? string.Empty;
        _levelOneText = levels.FirstOrDefault(x => x.Level == 1)?.Value ?? string.Empty;
        _levelTwoText = levels.FirstOrDefault(x => x.Level == 2)?.Value ?? string.Empty;
        _levelThreeText = levels.FirstOrDefault(x => x.Level == 3)?.Value ?? string.Empty;
        _levelFourText = levels.FirstOrDefault(x => x.Level == 4)?.Value ?? string.Empty;
    }
}