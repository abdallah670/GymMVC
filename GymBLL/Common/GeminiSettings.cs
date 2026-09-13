using System.ComponentModel.DataAnnotations;

namespace GymBLL.Common
{
    public class AISettings
    {
        [Required]
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-1.5-turbo";

        public int MaxTokens { get; set; } = 4096;
        public int Temperature { get; set; } = 0;
        public double TopP { get; set; } = 1.0;
        public int FrequencyPenalty { get; set; } = 0;
        public int PresencePenalty { get; set; } = 0;


        /// <summary>Returns true when a real API key has been supplied (i.e. not a placeholder).</summary>
        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(ApiKey) && !ApiKey.StartsWith("YOUR_");
    }
}
