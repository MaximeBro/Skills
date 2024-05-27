using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Skills.Databases;
using Skills.Models;
using Skills.Models.Enums;

namespace Skills.Services;

public class UserService(IConfiguration configuration, IDbContextFactory<SkillsContext> factory)
{
    
    public bool HasRequiredPermission(AuthenticationState state, UserModel user, UserRole[] roles)
    {
        if (state.User.Identity is { IsAuthenticated: true })
        {
            var email = state.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var role = state.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            return email == user.Email || roles.Select(x => x.ToString()).Contains(role);
        }

        return false;
    }
}