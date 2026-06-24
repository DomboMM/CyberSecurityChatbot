using System;

namespace CyberSecurityChatbotGUI.Models
{
    public class TaskItem
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            string status = IsCompleted ? "✅ Done" : "⏳ Pending";
            string reminder = ReminderDate.HasValue
                ? $" | Reminder: {ReminderDate.Value:dd MMM yyyy}"
                : "";

            return $"[{status}] {Title} — {Description}{reminder}";
        }
    }
}