using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CyberSecurityChatbotGUI.Services
{
    public enum Intent
    {
        AddTask,
        SetReminder,
        ViewTasks,
        CompleteTask,
        DeleteTask,
        StartQuiz,
        ShowActivityLog,
        Unknown
    }

    public class NlpHelper
    {
        private readonly Dictionary<Intent, string[]> _intentKeywords = new Dictionary<Intent, string[]>
        {
            { Intent.AddTask, new[] { "add a task", "add task", "create a task", "new task", "add to do", "add todo" } },
            { Intent.SetReminder, new[] { "remind me", "set a reminder", "set reminder", "reminder for" } },
            { Intent.ViewTasks, new[] { "show my tasks", "view tasks", "show tasks", "list tasks", "my tasks", "what tasks" } },
            { Intent.CompleteTask, new[] { "mark as done", "mark complete", "complete task", "finished task", "done with" } },
            { Intent.DeleteTask, new[] { "delete task", "remove task", "cancel task" } },
            { Intent.StartQuiz, new[] { "start quiz", "play quiz", "take quiz", "begin quiz", "test my knowledge", "quiz me" } },
            { Intent.ShowActivityLog, new[] { "show activity log", "activity log", "what have you done", "show log", "recent actions", "show history" } },
        };

        public Intent DetectIntent(string input)
        {
            string normalised = NormaliseInput(input);

            foreach (var pair in _intentKeywords)
            {
                if (pair.Value.Any(keyword => normalised.Contains(keyword)))
                {
                    return pair.Key;
                }
            }

            return Intent.Unknown;
        }

        private string NormaliseInput(string input)
        {
            string lower = input.ToLower().Trim();
            lower = Regex.Replace(lower, @"[^\w\s]", "");
            lower = Regex.Replace(lower, @"\s+", " ");
            return lower;
        }

        public string ExtractTaskTitle(string input)
        {
            string lower = input.ToLower();

            string[] triggers = { "to ", "task to ", "task -", "task:", "task ", "reminder to ", "remind me to " };

            foreach (var trigger in triggers)
            {
                int idx = lower.IndexOf(trigger);
                if (idx >= 0)
                {
                    string extracted = input.Substring(idx + trigger.Length).Trim();
                    if (!string.IsNullOrWhiteSpace(extracted))
                        return CapitaliseFirst(extracted.TrimEnd('.', '!', '?'));
                }
            }

            return CapitaliseFirst(input.Trim());
        }

        public DateTime? ExtractReminderDate(string input)
        {
            string lower = input.ToLower();

            if (lower.Contains("tomorrow"))
                return DateTime.Now.AddDays(1);

            if (lower.Contains("next week"))
                return DateTime.Now.AddDays(7);

            var match = Regex.Match(lower, @"in (\d+)\s*(day|days|week|weeks)");
            if (match.Success)
            {
                int amount = int.Parse(match.Groups[1].Value);
                string unit = match.Groups[2].Value;

                return unit.StartsWith("week")
                    ? DateTime.Now.AddDays(amount * 7)
                    : DateTime.Now.AddDays(amount);
            }

            return null;
        }

        private string CapitaliseFirst(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return char.ToUpper(text[0]) + text.Substring(1);
        }
    }
}