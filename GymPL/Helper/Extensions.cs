using System.Security.Claims;

namespace GymWeb.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return 
                   principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetUserFullName(this ClaimsPrincipal principal)
        {
            // First try to get from custom FullName claim
            var fullName = principal.FindFirst("DisplayName")?.Value;

            // If not found, try to get from Name claim (but it might be email)
            if (string.IsNullOrEmpty(fullName))
            {
                var nameClaim = principal.FindFirst(ClaimTypes.Name)?.Value;

                // If Name claim contains email, extract name part
                if (!string.IsNullOrEmpty(nameClaim) && nameClaim.Contains("@"))
                {
                    var nameFromEmail = nameClaim.Split('@')[0];
                    fullName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nameFromEmail);
                }
                else
                {
                    fullName = nameClaim ?? "User";
                }
            }

            return fullName;
        }

        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("Email")?.Value ??
                   principal.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string GetUserPhone(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("Phone")?.Value;
        }

        public static string GetUserType(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("Role")?.Value;
        }

        public static string GetProfilePicture(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("ProfilePicture")?.Value;
        }

        public static bool IsTrainer(this ClaimsPrincipal principal)
        {
            return principal.GetUserType() == "Trainer" || principal.IsInRole("Trainer");
        }

        public static bool IsMember(this ClaimsPrincipal principal)
        {
            return principal.GetUserType() == "Member" || principal.IsInRole("Member");
        }

        public static int GetTrainerExperienceYears(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirst("ExperienceYears")?.Value;
            return int.TryParse(value, out int years) ? years : 0;
        }

        public static string GetTrainerBio(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("Bio")?.Value ?? "";
        }

        public static string GetFitnessGoals(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("FitnessGoals")?.Value ?? "";
        }

        public static string GetMembershipType(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("MembershipType")?.Value ?? "";
        }

        public static DateTime? GetJoinDate(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirst("JoinDate")?.Value;
            if (DateTime.TryParse(value, out DateTime joinDate))
            {
                return joinDate;
            }
            return null;
        }

        public static int? GetCurrentWeight(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirst("CurrentWeight")?.Value;
            if (int.TryParse(value, out int currentWeight))
            {
                return currentWeight;
            }
            return null;
        }

        public static int? GetHeight(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirst("CurrentHeight")?.Value;
            if (int.TryParse(value, out int height))
            {
                return height;
            }
            return null;
        }

        public static bool IsUserActive(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirst("IsActive")?.Value;
            return bool.TryParse(value, out bool isActive) && isActive;
        }
    }
    public static class ClaimsExtensions
    {
        public static string FindFirstValue(this ClaimsPrincipal principal, string claimType)
        {
            return principal.FindFirst(claimType)?.Value;
        }
    }
}