using System.ComponentModel.DataAnnotations;

namespace GymBLL.Common
{
    public class EmailSettings
    {
        [Required]
        public string SmtpHost { get; set; } = string.Empty;

        [Range(1, 65535)]
        public int SmtpPort { get; set; } = 587;

        [Required]
        public string SmtpUser { get; set; } = string.Empty;

        [Required]
        public string SmtpPass { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string FromEmail { get; set; } = string.Empty;

        public string FromName { get; set; } = "MenoPro Gym";

        /// <summary>Returns true when real credentials have been supplied (i.e. not placeholders).</summary>
        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(SmtpUser) && !SmtpUser.StartsWith("YOUR_") &&
            !string.IsNullOrWhiteSpace(SmtpPass) && !SmtpPass.StartsWith("YOUR_");
    }
}
