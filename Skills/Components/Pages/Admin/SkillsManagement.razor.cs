using Microsoft.AspNetCore.Components;
using Skills.Components.Pages.Admin.SkillsTabs;

namespace Skills.Components.Pages.Admin;

public partial class SkillsManagement : ComponentBase
{
    private SkillsMapping _skillsMapping = null!;
    
    /// <summary>
    /// Used to refresh the Mapping's table content after a skill type was modified or deleted because they're dependant.
    /// </summary>
    public async Task RefreshSkillsAsync()
    {
        await _skillsMapping.RefreshDataAsync();
    }
}