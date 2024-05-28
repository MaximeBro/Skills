namespace Skills.Services;

public class RealTimeUpdateService
{
    public delegate Task OnUpdateAsyncHandler(string component, Guid circuitId);
    public event OnUpdateAsyncHandler OnUpdateAsync = null!;

    public Task SendUpdateAsync(string component, Guid circuitId)
    {
        OnUpdateAsync?.Invoke(component, circuitId);
        return Task.CompletedTask;
    }
}