using System.Net.Security;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Skills.Services;

namespace Skills.Components.Components;

public partial class FullComponentBase : ComponentBase, IAsyncDisposable
{
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public ThemeManager ThemeManager { get; set; } = null!;
    [Inject] public LocalizationManager Lang { get; set; } = null!;

    private HubConnection? hubConnection;
    private Guid circuitId = Guid.NewGuid();

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            ThemeManager.OnPaletteChanged += Refresh;
            Lang.OnLanguageChanged += async() => await InvokeAsync(StateHasChanged);
        }
    }

    private void Refresh() => NavManager.Refresh(true);
    protected async Task InitSignalRAsync(string component, Func<Task> method)
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(NavManager.ToAbsoluteUri(SkillsHub.HubUrl), options =>
            {
                options.WebSocketConfiguration = sockets =>
                {
                    sockets.RemoteCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => true);
                };
            })
            .Build();
        
        hubConnection.On<string, Guid>(SkillsHub.HubMethod, (message, id) =>
        {
            if (id == circuitId) return;

            if (component == message)
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

    protected async Task SendUpdateAsync(string action) => await hubConnection!.InvokeAsync(SkillsHub.HubMethod, action, circuitId);

    public async ValueTask DisposeAsync()
    {
        if (hubConnection != null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}