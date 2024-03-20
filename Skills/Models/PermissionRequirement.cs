using Microsoft.AspNetCore.Authorization;
using Skills.Models.Enums;

namespace Skills.Models;

/// <summary>
/// Represents the default permission requirement.
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Gets the policy needed for the permission requirement.
    /// </summary>
    public PermissionPolicy Policy { get; }

    /// <summary>
    /// Creates a permissions requirement for the specified policy.
    /// </summary>
    /// <param name="policy">The required permission policy.</param>
    public PermissionRequirement(PermissionPolicy policy)
    {
        Policy = policy;
    }

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString()
    {
        return Policy.ToString();
    }
}