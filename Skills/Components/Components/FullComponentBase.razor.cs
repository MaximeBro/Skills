using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Skills.Services;

namespace Skills.Components.Components;

public partial class FullComponentBase : ComponentBase, IAsyncDisposable
{
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public ThemeManager ThemeManager { get; set; } = null!;

    private HubConnection? hubConnection;
    private Guid circuitId = Guid.NewGuid();

    private string[] supported = [];

    protected async Task InitSignalRAsync(string[] actions, Func<Task> method)
    {
        supported = actions;
        hubConnection = new HubConnectionBuilder().WithUrl(NavManager.ToAbsoluteUri(SkillsHub.HubUrl)).Build();
        hubConnection.On<string, Guid>(SkillsHub.HubMethod, (message, id) =>
        {
            // if (id == circuitId) return; // Same circuit, we don't need to perform any action

            if (supported.Contains(message))
            {
                InvokeAsync(async() =>
                {
                    await method.Invoke();
                    StateHasChanged();
                });
            }
        });
        
        hubConnection.ServerTimeout = TimeSpan.FromSeconds(30);
        hubConnection.HandshakeTimeout = TimeSpan.FromSeconds(30);
        hubConnection.KeepAliveInterval = TimeSpan.FromSeconds(30);

        await hubConnection.StartAsync();
    }

    protected async Task SendUpdateAsync(string action) => await hubConnection!.SendAsync(SkillsHub.HubMethod, action, circuitId);

    public async ValueTask DisposeAsync()
    {
        if (hubConnection != null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}