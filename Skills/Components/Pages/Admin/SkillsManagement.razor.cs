using Skills.Components.Components;
using Skills.Components.Pages.Admin.SkillsTabs;

namespace Skills.Components.Pages.Admin;

public partial class SkillsManagement : FullComponentBase
{
    private SkillsMapping _skillsMapping = null!;
    private SkillsTypes _skillsTypes = null!;
    private SkillsCustomization _skillsCustomization = null!;
    
    /// <summary>
    /// Used to refresh the Mapping's table content after a skill type was modified or deleted because they're dependant.
    /// </summary>
    public async Task RefreshSkillsAsync()
    {
        await _skillsMapping.RefreshDataAsync();
    }

    public async Task RefreshSkillsTypesAsync()
    {
        await _skillsTypes.RefreshPageDataAsync();
        await _skillsCustomization.RefreshPageDataAsync();
    }
}