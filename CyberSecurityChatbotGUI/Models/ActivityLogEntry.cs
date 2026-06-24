using System;

namespace CyberSecurityChatbotGUI.Models
{
    public class ActivityLogEntry
    {
        public int LogId { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"[{Timestamp:dd MMM HH:mm}] ({Category}) {Description}";
        }
    }
}
