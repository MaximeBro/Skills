using System.Net.Security;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Skills.Components.Layout;
using Skills.Services;

namespace Skills.Components.Components;

public partial class FullComponentBase : ComponentBase, IAsyncDisposable
{
    [CascadingParameter(Name = "MainLayout")] public MainLayout Layout { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public ThemeManager ThemeManager { get; set; } = null!;
    [Inject] public LocalizationManager Lang { get; set; } = null!;

    private HubConnection? _hubConnection;
    private Guid _circuitId = Guid.NewGuid();

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            Lang.OnLanguageChanged += InvokeStateHasChangedAsync;
        }
    }
    
    protected async Task InitSignalRAsync(string component, Func<Task> method)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(NavManager.ToAbsoluteUri(SkillsHub.HubUrl), options => options.WebSocketConfiguration = sockets =>
            {
                sockets.RemoteCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => true);
            })
            .Build();
        
        _hubConnection.On<string, Guid>(SkillsHub.HubMethod, (message, id) =>
        {
            if (id == _circuitId) return;

            if (component == message)
            {
                InvokeAsync(async() =>
                {
                    await method.Invoke();
                    StateHasChanged();
                });
            }
        });
        
        _hubConnection.ServerTimeout = TimeSpan.FromSeconds(30);
        _hubConnection.HandshakeTimeout = TimeSpan.FromSeconds(30);
        _hubConnection.KeepAliveInterval = TimeSpan.FromSeconds(30);

        await _hubConnection.StartAsync();
    }

    protected async Task SendUpdateAsync(string action) => await _hubConnection!.InvokeAsync(SkillsHub.HubMethod, action, _circuitId);

    public async Task InvokeStateHasChangedAsync() => await InvokeAsync(StateHasChanged);

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            Lang.OnLanguageChanged -= InvokeStateHasChangedAsync;
            await _hubConnection.DisposeAsync();
        }
    }
}