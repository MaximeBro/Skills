using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Skills.Databases;
using Skills.Models;
using Skills.Models.Enums;

namespace Skills.Services;

public class PermissionHandler(IDbContextFactory<SkillsContext> factory) : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.PendingRequirements.Any())
        {
            var db = await factory.CreateDbContextAsync();
            var email = context.User.Identity?.Name ?? string.Empty;
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
            if (user is null) return;
            
            foreach (var requirement in context.PendingRequirements.OfType<PermissionRequirement>())
            {
                if ((user.Policy & PermissionPolicy.Root) == PermissionPolicy.Root)
                {
                    context.Succeed(requirement);
                }
                else if ((user.Policy & requirement.Policy) == requirement.Policy)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}