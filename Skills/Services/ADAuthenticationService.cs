using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Skills.Components.Dialogs;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models.Enums;

namespace Skills.Services;

[SuppressMessage("Interoperability", "CA1416:Valider la compatibilité de la plateforme")]
public class ADAuthenticationService(IConfiguration configuration, IDbContextFactory<SkillsContext> factory, UserTokenHoldingService userTokenHoldingService,
                                     IDialogService dialogService, NavigationManager navManager)
{
    public event NotAuthorizedEventHandler? OnNotAuthorized;
    public delegate Task NotAuthorizedEventHandler();

    public Task InvokeNotAuthorized()
    {
        OnNotAuthorized?.Invoke();
        return Task.CompletedTask;
    }
    
    public async Task<Guid?> TryLoginAsync(string username, string pwd)
    {
        var context = new PrincipalContext(ContextType.Domain, configuration["ip"]);
        var validated = context.ValidateCredentials(username, pwd);

        if (validated)
        {
            var db = await factory.CreateDbContextAsync();
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == $"{username}@sasp.fr");
            if (user != null)
            {
                var claims = new []
                {
                    new Claim("name", user.Name),
                    new Claim(ClaimTypes.Email, $"{username}@sasp.fr"),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };
                
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                var guid = Guid.NewGuid();
                userTokenHoldingService.PendingIdentities.Add(guid, claimsPrincipal);
                return guid;
            }

            return Guid.Empty; // The user is registered in the local AD but isn't saved in database yet
        }

        return null;
    }

    public async Task<bool> IsAuthenticatedAsync(Task<AuthenticationState> stateTask)
    {
        var authState = await stateTask;
        return authState.User.Identity?.IsAuthenticated ?? false;
    }
    
    public async Task<bool> HasRequiredRoleAsync(Task<AuthenticationState> stateTask, UserRole role)
    {
        var authState = await stateTask;
        var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var db = await factory.CreateDbContextAsync();
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        await db.DisposeAsync();

        if (user != null)
        {
            return user.Role == role || user.Role == UserRole.Admin;
        }

        return false;
    }
}