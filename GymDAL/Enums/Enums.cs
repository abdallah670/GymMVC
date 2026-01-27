namespace GymDAL.Enums
{
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


}