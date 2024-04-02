using Microsoft.AspNetCore.Components;
using Skills.Components.Components;
using Skills.Components.Pages.Admin.UsersTabs;

namespace Skills.Components.Pages.Admin;

public partial class UsersManagement : FullComponentBase
{
    private UsersList _users = null!;

    /// <summary>
    /// Used to refresh the Users table content after a group was modified or deleted because they're dependant.
    /// </summary>
    public async Task RefreshUsersAsync()
    {
        await _users.RefreshDataAsync();
    }
}