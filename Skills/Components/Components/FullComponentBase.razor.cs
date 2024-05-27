using Microsoft.AspNetCore.Components;
using Skills.Components.Layout;
using Skills.Services;

namespace Skills.Components.Components;

public partial class FullComponentBase : ComponentBase, IAsyncDisposable
{
    [CascadingParameter(Name = "MainLayout")] public MainLayout Layout { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public ThemeManager ThemeManager { get; set; } = null!;
    [Inject] public LocalizationManager Lang { get; set; } = null!;
    
    protected Guid CircuitId = Guid.NewGuid();
    private string _component = string.Empty;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            Lang.OnLanguageChanged += InvokeStateHasChangedAsync;
        }
    }

    protected async Task InvokeStateHasChangedAsync() => await InvokeAsync(StateHasChanged);

    protected virtual Task RefreshDataAsync()
    {
        return Task.CompletedTask;
    }

    public virtual async ValueTask DisposeAsync()
    {
        Lang.OnLanguageChanged -= InvokeStateHasChangedAsync;
    }
}