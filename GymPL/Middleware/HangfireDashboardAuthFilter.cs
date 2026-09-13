using Hangfire.AspNetCore;
using Hangfire.Dashboard;

namespace GymPL.Middleware
{
    /// <summary>
    /// Restricts the Hangfire dashboard to privileged roles only.
    /// By default the Hangfire dashboard allows any local request; this filter
    /// additionally requires an authenticated user in the Admin or Manager role.
    /// </summary>
    public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
    {
        private static readonly string[] AllowedRoles = { "Admin", "Manager" };

        public bool Authorize(DashboardContext context)
        {
            if (context is not AspNetCoreDashboardContext aspNetCoreContext)
                return false;

            var user = aspNetCoreContext.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                return false;

            foreach (var role in AllowedRoles)
            {
                if (user.IsInRole(role))
                    return true;
            }

            return false;
        }
    }
}
