using System.ComponentModel.DataAnnotations;

namespace GymBLL.Common
{
    public class StripeSettings
    {
        [Required]
        public string PublishableKey { get; set; } = string.Empty;

        [Required]
        public string SecretKey { get; set; } = string.Empty;

        [Required]
        public string WebhookSecret { get; set; } = string.Empty;

        /// <summary>Returns true when real keys have been supplied (i.e. not placeholders).</summary>
        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(SecretKey) && !SecretKey.StartsWith("YOUR_");
    }
}
