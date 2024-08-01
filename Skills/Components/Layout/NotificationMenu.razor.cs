using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Components;
using Skills.Databases;
using Skills.Models;
using Skills.Models.Enums;

namespace Skills.Components.Layout;

public partial class NotificationMenu : FullComponentBase, IDisposable
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Parameter] public NavBar Parent { get; set; } = null!;
    
    [Inject] public IDbContextFactory<SkillsContext> Factory { get; set; } = null!;
    
    private List<UserNotification> _notifications = [];

    private ClaimsPrincipal? _user;
    private string _username = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationState;
        _user = authState.User;
        _username = _user.FindFirstValue("username") ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(_username))
        {
            await RefreshDataAsync();
        }

        UpdateService.OnNotificationUpdateAsync += RefreshNotificationsAsync;
    }

    private async Task RefreshNotificationsAsync(string target)
    {
        if (target != _username) return;
        
        await InvokeAsync(async () =>
        {
            await RefreshDataAsync();
            StateHasChanged();
        });
    }
    
    private async Task MarkAsDoneAsync(params UserNotification[] notifications)
    {
        await using var db = await Factory.CreateDbContextAsync();
        db.Notifications.RemoveRange(notifications);
        await db.SaveChangesAsync();
        await RefreshDataAsync();
    }
    
    public override async Task RefreshDataAsync()
    {
        await using var db = await Factory.CreateDbContextAsync();
        _notifications = await db.Notifications.Where(x => x.Recipient!.Username == _username).OrderBy(x => x.CreatedAt).ToListAsync();
        Parent.SetNotificationsCount(_notifications.Count);
        StateHasChanged();
    }
    
    public void Dispose()
    {
        UpdateService.OnNotificationUpdateAsync -= RefreshNotificationsAsync;
    }
    
    // Notifications severity utils
    private Color GetNotificationIconColor(UserNotification notif)
    {
        return notif.Severity switch
        {
            NotificationSeverity.Hint => Color.Info,
            NotificationSeverity.Warning => Color.Warning,
            _ => Color.Error // Urgent
        };
    }

    private string GetNotificationIcon(UserNotification notif)
    {
        return notif.Severity switch
        {
            NotificationSeverity.Hint => Icons.Material.Rounded.Notifications,
            NotificationSeverity.Warning => Icons.Material.Rounded.Warning,
            _ => Icons.Material.Rounded.PriorityHigh // Urgent
        };
    }
}