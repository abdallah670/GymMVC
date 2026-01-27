namespace GymPL.Helper
{
    public static class Helper
    {
        public static string GetRelativeTime(System.DateTime dateTime)
        {
            var timeSpan = System.DateTime.UtcNow - dateTime;

            if (timeSpan <= System.TimeSpan.FromSeconds(60))
                return "Just now";
            if (timeSpan <= System.TimeSpan.FromMinutes(60))
                return timeSpan.Minutes > 1 ? $"{timeSpan.Minutes} minutes ago" : "a minute ago";
            if (timeSpan <= System.TimeSpan.FromHours(24))
                return timeSpan.Hours > 1 ? $"{timeSpan.Hours} hours ago" : "an hour ago";
            if (timeSpan <= System.TimeSpan.FromDays(30))
                return timeSpan.Days > 1 ? $"{timeSpan.Days} days ago" : "yesterday";
            if (timeSpan <= System.TimeSpan.FromDays(365))
                return timeSpan.Days > 30 ? $"{timeSpan.Days / 30} months ago" : "a month ago";

            return timeSpan.Days > 365 ? $"{timeSpan.Days / 365} years ago" : "a year ago";
        }
    }
}
