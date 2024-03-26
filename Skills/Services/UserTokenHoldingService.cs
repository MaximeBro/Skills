using System.Security.Claims;

namespace Skills.Services;

public class UserTokenHoldingService
{
    public Dictionary<Guid, ClaimsPrincipal> PendingIdentities { get; } = [];
}