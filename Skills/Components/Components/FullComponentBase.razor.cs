using Microsoft.AspNetCore.Components;
using Skills.Components.Layout;
using Skills.Services;

namespace Skills.Components.Components;

public partial class FullComponentBase : ComponentBase, IAsyncDisposable
{
    [CascadingParameter(Name = "MainLayout")] public MainLayout Layout { get; set; } = null!;
    [Inject] public RealTimeUpdateService UpdateService { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public ThemeManager ThemeManager { get; set; } = null!;
    [Inject] public LocalizationManager Lang { get; set; } = null!;
    
    protected Guid CircuitId = Guid.NewGuid();
    protected string Component = string.Empty;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            Component = this.GetType().Name;
            Lang.OnLanguageChanged += InvokeStateHasChangedAsync;
            UpdateService.OnUpdateAsync += RefreshComponentDataAsync;
        }
    }

    protected async Task InvokeStateHasChangedAsync() => await InvokeAsync(StateHasChanged);

    protected async Task SendUpdateAsync() => await UpdateService.SendUpdateAsync(Component, CircuitId);
    protected async Task SendNotificationUpdateAsync(string target) => await UpdateService.SendNotificationUpdateAsync(target);

    private async Task RefreshComponentDataAsync(string component, Guid circuitId)
    {
        if (circuitId != CircuitId && component == Component)
        {
            await InvokeAsync(async () =>
            {
                await RefreshDataAsync();
                StateHasChanged();
            });
        }
    }
    
    public virtual Task RefreshDataAsync() => Task.CompletedTask;

    public virtual ValueTask DisposeAsync()
    {
        Lang.OnLanguageChanged -= InvokeStateHasChangedAsync;
        UpdateService.OnUpdateAsync -= RefreshComponentDataAsync;

        return new ValueTask(Task.CompletedTask);
    }
}