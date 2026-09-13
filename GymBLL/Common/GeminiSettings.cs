using System.ComponentModel.DataAnnotations;

namespace GymBLL.Common
{
    public class GeminiSettings
    {
        [Required]
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>Returns true when a real API key has been supplied (i.e. not a placeholder).</summary>
        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(ApiKey) && !ApiKey.StartsWith("YOUR_");
    }
}
