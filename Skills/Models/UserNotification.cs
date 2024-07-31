using Skills.Models.Enums;

namespace Skills.Models;

public class UserNotification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public Guid SenderId { get; set; }
    public UserModel? Sender { get; set; }
    
    public Guid RecipientId { get; set; }
    public UserModel? Recipient { get; set; }
    
    public NotificationSeverity Severity { get; set; } = NotificationSeverity.Hint;

    public string Content { get; set; } = string.Empty;
}