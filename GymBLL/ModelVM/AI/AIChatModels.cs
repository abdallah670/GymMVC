using System.Collections.Generic;

namespace GymBLL.ModelVM.AI
{
    public class AIChatRequest
    {
        public string Message { get; set; }
        // We can add history here later for context awareness
    }

    public class AIChatResponse
    {
        public string Reply { get; set; }
        public List<string> SuggestedActions { get; set; } = new List<string>();
        public bool IsError { get; set; }
    }
}
