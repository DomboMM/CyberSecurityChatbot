using System.Collections.Generic;
using CyberSecurityChatbotGUI.Models;

namespace CyberSecurityChatbotGUI.Services
{
    public class ActivityLogService
    {
        private readonly DatabaseHelper _db;

        public ActivityLogService(DatabaseHelper db)
        {
            _db = db;
        }

        public void Log(string description, string category)
        {
            _db.AddLogEntry(description, category);
        }

        public List<ActivityLogEntry> GetRecent(int count = 10)
        {
            return _db.GetRecentLogs(count);
        }

        public string GetFormattedSummary(int count = 10)
        {
            var logs = GetRecent(count);

            if (logs.Count == 0)
                return "No activity has been recorded yet.";

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("📋 Here's a summary of recent actions:");

            int i = 1;
            foreach (var entry in logs)
            {
                sb.AppendLine($"{i}. {entry.Description} ({entry.Timestamp:dd MMM, HH:mm})");
                i++;
            }

            return sb.ToString();
        }
    }
}