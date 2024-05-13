using Microsoft.AspNetCore.SignalR;

namespace Skills.Services;

public class SkillsHub : Hub
{
    public const string HubUrl = "/api/refresh";
    public const string HubMethod = "ReceiveMessage";

    public async Task BroadcastAsync(string message, Guid circuitId)
    {
        await Clients.All.SendAsync(HubMethod, message, circuitId);
    }
}