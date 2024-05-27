using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Models;

namespace Skills.Components.Pages.UsersPages;

public partial class UsersOverview : FullComponentBase
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;

    [Parameter] public string Id { get; set; } = null!;
    [Parameter] public int? TabIndex { get; set; }
    
    private UserModel _user = null!;
    
    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
    }

    public async Task UserChangedAsync()
    {
        await RefreshDataAsync();
        StateHasChanged();
    }
    
    protected override async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username.ToLower().Contains(Id.ToLower()));
        
        if (user is null) NavManager.NavigateTo("/", true);
        else _user = user;
    }
}