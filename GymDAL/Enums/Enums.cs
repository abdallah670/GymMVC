// Enums.cs
public enum UserType
{
    Admin,
    Trainer,
    Member
}

public enum WorkoutStatus
{
    Pending,
    Completed,
    Skipped,
    Incomplete
}

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
public enum ReportType
{
    MemberProgress,
    TrainerPerformance,
    Financial,
    Attendance,
    WorkoutCompletion,
    DietAdherence,
    SystemUsage,
    Custom
}

public enum ReportStatus
{
    Pending,
    Generating,
    Completed,
    Failed,
    Cancelled
}

// Base entity for common properties
public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;


    public DateTime? DeletedAt { get; set; } = DateTime.UtcNow;


    public string? CreatedBy { get; set; }
    public string? DeletedBy { get; set; }

    public string? UpdatedBy { get; set; }


    public bool IsActive { get; set; } = true;
}