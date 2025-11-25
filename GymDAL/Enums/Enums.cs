
public enum WorkoutStatus
{
    Pending,
    Completed,
    Skipped,
    Incomplete
}
public enum SubscriptionStatus { Active, Expired, Cancelled,Freezed }

public enum NotificationType
{
    WorkoutReminder,
    MealReminder,
    ProgressAlert,
    SystemAlert,
    AssignmentAlert
}

public enum NotificationStatus
{
    Unread,
    Read,
    Archived
}

public enum DeliveryMethod
{
    Email,
    InApp,
    Both
}

// Base entity for common properties
public abstract class BaseEntity
{
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; } = null;
}