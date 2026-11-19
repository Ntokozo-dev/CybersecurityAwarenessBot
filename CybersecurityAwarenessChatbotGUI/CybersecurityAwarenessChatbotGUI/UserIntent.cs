using System;
using System.Text.RegularExpressions;


namespace CybersecurityAwarenessChatbotGUI
{
    // Represents what the user wants to do, based on their typed message.
    public enum UserIntent
    {
        None,
        StartQuiz,
        ShowTasks,
        AddTask,
        ShowActivityLog,
        AskCybersecurityTopic
    }

    // Looks at cleaned user input and decides what they actually want,
    // so the GUI can trigger the right feature.
    public class IntentRecognizer
    {
        public UserIntent DetectIntent(string input)
        {
            string text = input.ToLower().Trim();

            // Checked first so phrases like "show my quiz activity" count as an activity-log request.
            if (text.Contains("activity log") || text.Contains("show activity") || text.Contains("view activity") ||
                text.Contains("recent activity") || text.Contains("my activity") || text.Contains("recent actions") ||
                text.Contains("what have you done") || text.Contains("what did i do") ||
                text.Contains("show log") || text.Contains("view log") || text.Contains("action log"))
            {
                return UserIntent.ShowActivityLog;
            }

            if (text.Contains("quiz") || text.Contains("test my knowledge") || text.Contains("start quiz"))
            {
                return UserIntent.StartQuiz;
            }

            if (text.Contains("my tasks") || text.Contains("show tasks") || text.Contains("view tasks") || text.Contains("task list"))
            {
                return UserIntent.ShowTasks;
            }

            if (text.Contains("add a task") || text.Contains("add task") || text.Contains("create a task") ||
                text.Contains("create task") || text.Contains("new task") || text.Contains("remind me"))
            {
                return UserIntent.AddTask;
            }

            if (text.Contains("password") || text.Contains("phishing") || text.Contains("privacy") || text.Contains("scam"))
            {
                return UserIntent.AskCybersecurityTopic;
            }

            return UserIntent.None;
        }

        // Strips the command words and any reminder wording, leaving just the task title.
        // "Remind me to update my password tomorrow." -> "update my password"
        public string ExtractTaskTitle(string input)
        {
            // Remove common command phrases from the start.
            string title = Regex.Replace(
                input.Trim(),
                @"^(please\s+)?(add\s+(a\s+)?task(\s+to\b|\s*-\s*)?|" +
                @"create\s+(a\s+)?task(\s+to\b|\s*-\s*)?|new\s+task(\s+to\b|\s*-\s*)?|" +
                @"remind\s+me(\s+to\b)?)\s*",
                "",
                RegexOptions.IgnoreCase);

            // Remove simple reminder wording from the end.
            title = Regex.Replace(
                title,
                @"(^|\s+)(today|tomorrow|next\s+week|in\s+\d+\s+days?)\.?$",
                "",
                RegexOptions.IgnoreCase);

            return title.Trim().TrimEnd('.');
        }

        // Looks for simple reminder wording ("today", "tomorrow", "next week", "in 3 days").
        // Returns null when the message has no reminder.
        public DateTime? ExtractReminderDate(string input)
        {
            string text = input.ToLower();

            if (text.Contains("tomorrow"))
            {
                return DateTime.Today.AddDays(1);
            }

            if (text.Contains("next week"))
            {
                return DateTime.Today.AddDays(7);
            }

            Match match = Regex.Match(text, @"in\s+(\d+)\s+days?");
            if (match.Success)
            {
                int days = Convert.ToInt32(match.Groups[1].Value);
                return DateTime.Today.AddDays(days);
            }

            if (Regex.IsMatch(text, @"\btoday\b"))
            {
                return DateTime.Today;
            }

            return null;
        }
    }
}
