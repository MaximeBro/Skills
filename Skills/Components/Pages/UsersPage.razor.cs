using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Databases;
using Skills.Models;
using Icons = MudBlazor.Icons.Material.Filled;

namespace Skills.Components.Pages;

public partial class UsersPage
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;

    private List<UserModel> _users = new();

    private string _search = string.Empty;
    private Func<UserModel, bool> QuickFilter => x =>
    {
        if (x.Name.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Username.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Email.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
            
        return false;
    };

    private int _toggleIndex = 1;
    private string _toggleIcon => _toggleIndex < 2 ? Icons.GridView : _toggleIndex == 2 ? Icons.ViewList : Icons.TableChart;
    private string _toggleTooltip => _toggleIndex < 2 ? "Cartes" : _toggleIndex == 2 ? "Liste" : "Tableau";
    private void ToggleView() => _toggleIndex += _toggleIndex == 3 ? -2 : 1;

    protected override async Task OnInitializedAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _users = await db.Users.AsNoTracking().ToListAsync();
    }

    private void OnRowClicked(DataGridRowClickEventArgs<UserModel> args)
    {
        if (args.MouseEventArgs.Detail == 2)
        {
            NavManager.NavigateTo($"/overview/{args.Item.Username}");
        }
    }
}