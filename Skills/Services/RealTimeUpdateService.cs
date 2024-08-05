using System.Diagnostics;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Models.Enums;
using Skills.Models.Overview;
using Timer = System.Timers.Timer;

namespace Skills.Services;

/// <summary>
/// This service is used to notify page components that a circuit has sent an update to the same component.
/// This is a small workaround for real-time updates instead of using SignalR. This service shouldn't be used for fast updates.
/// </summary>
public class RealtimeUpdateService(ILogger<RealtimeUpdateService> logger, IDbContextFactory<SkillsContext> factory) : IDisposable
{
    public delegate Task OnUpdateAsyncHandler(string component, Guid circuitId);
    public event OnUpdateAsyncHandler OnUpdateAsync = null!;

    public delegate Task OnNotificationUpdateAsyncHandler(string target);
    public event OnNotificationUpdateAsyncHandler OnNotificationUpdateAsync = null!;

    private readonly Timer _certificationTimer = new Timer(1000 * 60 * 60 * 24); // Every day

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

    public async Task InitAsync()
    {
        _certificationTimer.Elapsed += async (_, _) => await CheckExpiredCertificationsAsync();
        _certificationTimer.Start();
        await CheckExpiredCertificationsAsync(); // We force it when the server starts
    }

    private async Task CheckExpiredCertificationsAsync()
    {
        logger.LogInformation("{name} started !", nameof(CheckExpiredCertificationsAsync));
        var st = Stopwatch.StartNew();
        
        await using var db = await factory.CreateDbContextAsync();
        var certs = await db.UserSafetyCertifications.AsNoTracking()
                                                                               .Include(x => x.Certification)
                                                                               .Include(x => x.User)
                                                                               .ToListAsync();
        var toDell = certs.Where(x => x.ExpireDate.HasValue && x.ExpireDate <= DateTime.Now).ToList();

        var notifs = toDell.Select(x =>
            new UserNotification {
                RecipientId = x.UserId,
                Content = $"Votre habilitation {x.Certification!.Name} de type {x.Certification!.Category} a expiré !\nVeuillez la renouveler sur votre espace personnel.",
                Severity = NotificationSeverity.Warning
            }
        ).ToList();

        await db.Notifications.AddRangeAsync(notifs);
        await db.SaveChangesAsync();

        // We send notifications to concerned users (if they are not online they will receive it when they will be online again)
        foreach (var cert in toDell.DistinctBy(x => x.UserId))
        {
            await SendNotificationUpdateAsync(cert.User!.Username);
        }
        
        db.UserSafetyCertifications.RemoveRange(toDell.Select(x => new UserSafetyCertificationInfo { Id = x.Id, UserId = x.UserId, CertId = x.CertId }));
        
        st.Stop();
        logger.LogInformation("{name} finished in {time} !", nameof(CheckExpiredCertificationsAsync), st.Elapsed.Humanize(culture: Hardcoded.English, precision: 2));
    }

    public void Dispose()
    {
        _certificationTimer.Dispose();
    }
}