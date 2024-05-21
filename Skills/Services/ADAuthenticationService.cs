using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Skills.Databases;
using Skills.Models.Enums;

namespace Skills.Services;

[SuppressMessage("Interoperability", "CA1416:Valider la compatibilité de la plateforme")]
public class ADAuthenticationService(IConfiguration configuration, IDbContextFactory<SkillsContext> factory, UserTokenHoldingService userTokenHoldingService)
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
        var context = new PrincipalContext(ContextType.Domain, configuration["ip"],
                                                            configuration.GetSection("DirectoryServices")["login"], 
                                                            configuration.GetSection("DirectoryServices")["password"]);
        var validated = context.ValidateCredentials(username, pwd, ContextOptions.Negotiate);

        if (validated)
        {
            var email = username == "anthony" ? "anthony.simon@sasp.fr" : $"{username}@sasp.fr";
            var db = await factory.CreateDbContextAsync();
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {
                var claims = new[]
                {
                    new Claim("name", user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
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
    
    public async Task<bool> HasRequiredRoleAsync(Task<AuthenticationState> stateTask, string[] roles)
    {
        var authState = await stateTask;
        var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var db = await factory.CreateDbContextAsync();
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        await db.DisposeAsync();

        if (user != null)
        {
            return roles.Contains(user.Role.ToString(), StringComparer.OrdinalIgnoreCase) || user.Role == UserRole.Admin;
        }

        return false;
    }
}