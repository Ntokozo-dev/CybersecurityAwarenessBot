using System;


namespace CybersecurityAwarenessChatbotGUI
{
    // Represents what the user wants to do, based on their typed message.
    public enum UserIntent
    {
        None,
        StartQuiz,
        ShowTasks,
        AddTask,
        AskCybersecurityTopic
    }

    // Looks at cleaned user input and decides what they actually want,
    // so the GUI can trigger the right feature.
    public class IntentRecognizer
    {
        public UserIntent DetectIntent(string input)
        {
            string text = input.ToLower().Trim();

            if (text.Contains("quiz") || text.Contains("test my knowledge") || text.Contains("start quiz"))
            {
                return UserIntent.StartQuiz;
            }

            if (text.Contains("my tasks") || text.Contains("show tasks") || text.Contains("view tasks") || text.Contains("task list"))
            {
                return UserIntent.ShowTasks;
            }

            if (text.Contains("add a task") || text.Contains("new task") || text.Contains("remind me"))
            {
                return UserIntent.AddTask;
            }

            if (text.Contains("password") || text.Contains("phishing") || text.Contains("privacy") || text.Contains("scam"))
            {
                return UserIntent.AskCybersecurityTopic;
            }

            return UserIntent.None;
        }
    }
}