using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
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
        try
        {
            var endpoint =
                new LdapDirectoryIdentifier(configuration.GetSection("DirectoryServices")["ip"], false, false);
            var ldap = new LdapConnection(endpoint, new NetworkCredential(username, pwd))
            {
                AuthType = AuthType.Negotiate
            };
            ldap.Bind();

            var db = await factory.CreateDbContextAsync();
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == $"{username}@sasp.fr");
            if (user != null)
            {
                var claims = new[]
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
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
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