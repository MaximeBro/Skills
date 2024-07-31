namespace Skills.Services;

/// <summary>
/// This service is used to notify page components that a circuit has sent an update to the same component.
/// This is a small workaround for real-time updates instead of using SignalR. This service shouldn't be used for fast updates.
/// </summary>
public class RealTimeUpdateService
{
    public delegate Task OnUpdateAsyncHandler(string component, Guid circuitId);
    public event OnUpdateAsyncHandler OnUpdateAsync = null!;

    public delegate Task OnNotificationUpdateAsyncHandler(string target);
    public event OnNotificationUpdateAsyncHandler OnNotificationUpdateAsync = null!;

    /// <summary>
    /// Sends an update to all circuits subscribed to the <see cref="OnUpdateAsync"/> event with the component name that has been changed and the sender circuitId.
    /// </summary>
    /// <param name="component">The notified component.</param>
    /// <param name="circuitId">The author of the update.</param>
    /// <returns>A completed task.</returns>
    public Task SendUpdateAsync(string component, Guid circuitId)
    {
        OnUpdateAsync?.Invoke(component, circuitId);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sends an update to the given target.
    /// </summary>
    /// <param name="target">The username of the target.</param>
    /// <returns>A completed task.</returns>
    public Task SendNotificationUpdateAsync(string target)
    {
        OnNotificationUpdateAsync?.Invoke(target);
        return Task.CompletedTask;
    }
}