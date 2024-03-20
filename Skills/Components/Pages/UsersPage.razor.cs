using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Skills.Databases;
using Skills.Models;

namespace Skills.Components.Pages;

public partial class UsersPage
{
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;

    private List<UserModel> _users = new();
    
    protected override async Task OnInitializedAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _users = await db.Users.AsNoTracking().ToListAsync();
    }
}