using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CybersecurityAwarenessChatbotGUI
{
    public partial class MainWindow : Window
    {
        // Fields: these stay alive as long as the window is open (not recreated on each click).

        // Which "stage" of the conversation we are in: first we need the user's name.
        private bool waitingForName = true;

        // Stores and validates the user's name (carried over from Part 1).
        private readonly UserProfile user = new UserProfile();

        // The chatbot "brain".
        private readonly ResponseHandler responseHandler = new ResponseHandler();

        // Handles all MySQL reads/writes for the Task Assistant.
        private readonly TaskDatabase taskDb = new TaskDatabase();

        // Colours for the chat: the user's messages are white, the bot's are light green.
        private readonly Brush userBrush = Brushes.White;
        private readonly Brush botBrush = Brushes.LightGreen;

        // Detects what the user wants based on typed input (e.g. "start quiz").
        private readonly IntentRecognizer intentRecognizer = new IntentRecognizer();

        //stores every meaningful action(tasks, reminders, quiz and NLP commands)
        private readonly ActivityLogger activityLogger = new ActivityLogger();

        public MainWindow()
        {
            // Builds the window from the XAML (auto-generated step).
            InitializeComponent();

            // DELEGATE in use: we plug our DisplayBotMessage method into the chatbot.
            // When the chatbot has a reply, it calls this method without needing to know
            // anything about rtbChatHistory.
            responseHandler.MessageReady = DisplayBotMessage;

            // EVENT SUBSCRIPTION (also a delegate): run MainWindow_Loaded once the window has loaded.
            Loaded += MainWindow_Loaded;
        }

        // Runs once, when the window first appears.
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Voice greeting. Play() does not block, so the window stays responsive.
            VoiceGreeting greeting = new VoiceGreeting();
            greeting.PlayGreeting();

            // ASCII art goes in the banner box.
            AsciiArt art = new AsciiArt();
            tbBanner.Text = art.GetLogo();

            // Remove the empty paragraph a new RichTextBox starts with, so the chat begins at the top.
            rtbChatHistory.Document.Blocks.Clear();

            DisplayBotMessage("Welcome to the Cybersecurity Awareness Bot!");
            DisplayBotMessage("What is your name?");

            // Put the cursor in the input box so the user can start typing straight away.
            txtUserInput.Focus();

            // Load any tasks already saved in the database into the Tasks tab.
            RefreshTaskList();
            // Shows "No activity recorded yet." in the Activity Log tab until something is logged.
            DisplayRecentActivity(5);
            //checks for reminders as soon as the app opens
            CheckDueReminders();

        }



        // Runs every time the Send button is clicked (or Enter is pressed, because btnSend IsDefault).
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string input = txtUserInput.Text.Trim();

            // Blank input: give a helpful message instead of doing nothing.
            if (string.IsNullOrWhiteSpace(input))
            {
                DisplayBotMessage(waitingForName
                    ? "I didn't catch that. Please enter your name."
                    : "Please type a question or cybersecurity topic first.");
                txtUserInput.Focus();
                return;
            }

            // Show the user's own message first so both sides of the conversation are visible.
            AppendToChat("You: " + input, userBrush);

            if (waitingForName)
            {
                HandleNameInput(input);
            }
            else
            {
                // NLP SIMULATION: figure out what the user actually wants.
                UserIntent intent = intentRecognizer.DetectIntent(input);

                switch (intent)
                {
                    case UserIntent.StartQuiz:
                        DisplayBotMessage("Sure — switching you to the Quiz tab!");
                        QuizTabItem.IsSelected = true; // switches the active tab
                        LogAction("NLP recognised start-quiz request");
                        break;

                    case UserIntent.ShowTasks:
                        DisplayBotMessage("Here are your tasks — switching to the Tasks tab.");
                        TasksTabItem.IsSelected = true;
                        RefreshTaskList();
                        LogAction("NLP recognised show-tasks request");
                        break;

                    case UserIntent.AddTask:
                        // Creates the task straight from the chat message (title + optional reminder).
                        AddTaskFromChat(input);
                        break;

                    case UserIntent.ShowActivityLog:
                        DisplayBotMessage("Here is your recent activity — switching to the Activity Log tab.");
                        // Log first so this request itself appears (LogAction also refreshes the list).
                        LogAction("NLP recognised activity-log request");
                        ActivityTabItem.IsSelected = true;
                        break;

                    default:
                        // Fall back to your existing cybersecurity response logic.
                        responseHandler.ProcessInput(input, user.Name);
                        break;
                }
            }

            txtUserInput.Clear();
            txtUserInput.Focus();
        }

        // First stage of the conversation: capture and validate the user's name.
        private void HandleNameInput(string input)
        {
            // SetName validates AND stores the name; it returns true/false.
            if (user.SetName(CleanName(input)))
            {
                waitingForName = false;
                DisplayBotMessage($"Nice to meet you, {user.Name}! Ask me about " + responseHandler.GetTopicList() + ".");
            }
            else
            {
                DisplayBotMessage("I didn't catch that. Please enter your name.");
            }
        }

        // Lets people type "my name is Sam" or "I'm Sam" and still store just "Sam".
        private static string CleanName(string input)
        {
            string text = input.Replace('\u2019', '\'');
            string[] prefixes = { "my name is ", "i am ", "i'm ", "im ", "call me ", "it's ", "its " };

            foreach (string prefix in prefixes)
            {
                if (text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return text.Substring(prefix.Length).Trim();
                }
            }
            return text;
        }

        // This is the method the delegate points to: shows a bot message in the chat.
        private void DisplayBotMessage(string message)
        {
            AppendToChat("Bot: " + message, botBrush);
        }

        // One place that adds text to the chat, so colour and scrolling are handled once.
        private void AppendToChat(string message, Brush colour)
        {
            // Each message becomes its own paragraph with its own colour.
            Paragraph paragraph = new Paragraph(new Run(message));
            paragraph.Foreground = colour;
            rtbChatHistory.Document.Blocks.Add(paragraph);

            // Keep the newest message in view.
            rtbChatHistory.ScrollToEnd();
        }

        // ---------------------------------------------------------
        // TASK ASSISTANT (Tasks tab) — reads and writes via TaskDatabase
        // ---------------------------------------------------------

        // Reusable method: clears and repopulates the task list display
        // from the database, so it always shows the current state.
        private void RefreshTaskList()
        {
            lstTasks.Items.Clear();
            List<CyberTask> tasks = taskDb.GetAllTasks();

            foreach (CyberTask task in tasks)
            {
                string status = task.IsCompleted ? "[DONE]" : "[PENDING]";
                string reminderText = task.Reminder.HasValue
                    ? task.Reminder.Value.ToShortDateString()
                    : "No reminder";
                lstTasks.Items.Add($"{status} ID:{task.TaskId} - {task.Title} | {task.Description} | {reminderText}");
            }
        }
        // Checks all tasks for reminders that are due today or overdue,
        // and shows a pop-up alert listing them.
        private void CheckDueReminders()
        {
            List<CyberTask> tasks = taskDb.GetAllTasks();
            List<string> dueTasks = new List<string>();

            foreach (CyberTask task in tasks)
            {
                // Only check tasks that aren't already completed and actually have a reminder set.
                if (!task.IsCompleted && task.Reminder.HasValue)
                {
                    // .Date strips the time portion, so we're comparing just the calendar day.
                    if (task.Reminder.Value.Date <= DateTime.Today)
                    {
                        dueTasks.Add($"- {task.Title} (due {task.Reminder.Value.ToShortDateString()})");
                    }
                }
            }

            if (dueTasks.Count > 0)
            {
                string message = "You have reminders due:\n\n" + string.Join("\n", dueTasks);
                MessageBox.Show(message, "Task Reminders", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        // Runs when "Add Task" is clicked.
        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTaskTitle.Text;
            string description = txtTaskDescription.Text;
            string reminderInput = txtTaskReminder.Text;

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a title for the task.");
                return;
            }

            // Reminder is optional, so only try to parse it if something was typed.
            DateTime? reminder = null;
            if (!string.IsNullOrWhiteSpace(reminderInput))
            {
                if (DateTime.TryParse(reminderInput, out DateTime parsedDate))
                {
                    reminder = parsedDate;
                }
                else
                {
                    MessageBox.Show("Reminder date not recognised. Task added without a reminder.");
                }
            }

            taskDb.AddTask(title, description, reminder);

            // Record the new task (and its reminder, if one was set) in the Activity Log.
            LogAction($"Task added: {title}");
            if (reminder.HasValue)
            {
                LogAction($"Reminder set: {title} - {reminder.Value:dd MMM yyyy}");
            }

            // Clear the input fields and refresh the list to show the new task.
            txtTaskTitle.Clear();
            txtTaskDescription.Clear();
            txtTaskReminder.Clear();
            RefreshTaskList();
        }

        // Runs when "Mark Complete" is clicked.
        private void btnCompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (lstTasks.SelectedItem == null)
            {
                MessageBox.Show("Please select a task first.");
                return;
            }

            string selected = lstTasks.SelectedItem.ToString();
            int taskId = ExtractTaskId(selected);

            taskDb.CompleteTask(taskId);
            LogAction($"Task completed: {ExtractTaskTitle(selected)}");
            RefreshTaskList();
        }

        // Runs when "Delete" is clicked.
        private void btnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (lstTasks.SelectedItem == null)
            {
                MessageBox.Show("Please select a task first.");
                return;
            }

            string selected = lstTasks.SelectedItem.ToString();
            int taskId = ExtractTaskId(selected);

            taskDb.DeleteTask(taskId);
            LogAction($"Task deleted: {ExtractTaskTitle(selected)}");
            RefreshTaskList();
        }

        // Helper: pulls the TaskId number out of the displayed list item text,
        // since the ListBox only shows a formatted string, not the raw CyberTask object.
        private int ExtractTaskId(string displayText)
        {
            int idIndex = displayText.IndexOf("ID:") + 3;
            int spaceIndex = displayText.IndexOf(' ', idIndex);
            string idText = displayText.Substring(idIndex, spaceIndex - idIndex);
            return int.Parse(idText);
        }

        // Helper: pulls the task title out of the displayed list item text
        // (format is "[STATUS] ID:n - Title | Description | Reminder"), for the Activity Log.
        private string ExtractTaskTitle(string displayText)
        {
            int start = displayText.IndexOf(" - ") + 3;
            int end = displayText.IndexOf(" | ", start);
            return displayText.Substring(start, end - start);
        }
        // ---------------------------------------------------------
        // QUIZ (Quiz tab) — runs through QuizManager
        // ---------------------------------------------------------

        private readonly QuizManager quizManager = new QuizManager();

        // Runs when "Start Quiz" is clicked.
        private void btnQuizStart_Click(object sender, RoutedEventArgs e)
        {
            quizManager.Reset();
            btnQuizStart.IsEnabled = false;
            btnQuizSubmit.IsEnabled = true;
            LogAction("Cybersecurity quiz started");
            LoadQuizQuestion();
        }

        // Displays the current question and builds radio buttons for its options.
        private void LoadQuizQuestion()
        {
            QuizQuestion question = quizManager.GetCurrentQuestion();

            if (question == null)
            {
                ShowFinalScore();
                return;
            }

            tbQuizQuestion.Text = question.QuestionText;
            tbQuizFeedback.Text = "";
            spQuizOptions.Children.Clear();

            foreach (string option in question.Options)
            {
                RadioButton rb = new RadioButton
                {
                    Content = option,
                    GroupName = "QuizOptions",
                    Foreground = Brushes.LightGreen,
                    FontFamily = new FontFamily("Consolas"),
                    Margin = new Thickness(0, 5, 0, 5)
                };
                spQuizOptions.Children.Add(rb);
            }

            btnQuizSubmit.IsEnabled = true;
            btnQuizNext.IsEnabled = false;
        }

        // Runs when "Submit Answer" is clicked: checks the answer, shows feedback,
        // but does NOT move to the next question — that's a separate step now.
        private void btnQuizSubmit_Click(object sender, RoutedEventArgs e)
        {
            string selectedAnswer = null;
            foreach (var child in spQuizOptions.Children)
            {
                if (child is RadioButton rb && rb.IsChecked == true)
                {
                    selectedAnswer = rb.Content.ToString();
                    break;
                }
            }

            if (selectedAnswer == null)
            {
                MessageBox.Show("Please select an answer before submitting.");
                return;
            }

            QuizAnswerResult result = quizManager.SubmitAnswer(selectedAnswer);

            tbQuizFeedback.Text = result.IsCorrect
                ? "Correct!"
                : $"Incorrect. The correct answer was: {result.CorrectAnswer}";

            // Lock in this answer: disable Submit and the radio buttons,
            // enable Next so the user moves on when they're ready.
            btnQuizSubmit.IsEnabled = false;
            foreach (var child in spQuizOptions.Children)
            {
                if (child is RadioButton rb) rb.IsEnabled = false;
            }
            btnQuizNext.IsEnabled = true;
        }

        // Runs when "Next Question" is clicked.
        private void btnQuizNext_Click(object sender, RoutedEventArgs e)
        {
            LoadQuizQuestion();
        }

        // Shows the final score once all questions are done, out of 100 (percentage).
        private void ShowFinalScore()
        {
            int correct = quizManager.GetScore();
            int total = quizManager.GetTotalQuestions();

            // Convert raw score (e.g. 9/12) into a percentage out of 100.
            double percentage = (double)correct / total * 100;

            tbQuizQuestion.Text = "Quiz complete!";
            spQuizOptions.Children.Clear();
            tbQuizFeedback.Text = $"You scored {correct}/{total} — {percentage:F1}/100";

            LogAction($"Quiz completed - score {correct}/{total}");

            btnQuizSubmit.IsEnabled = false;
            btnQuizNext.IsEnabled = false;
            btnQuizStart.IsEnabled = true; // allow retaking the quiz
            btnQuizStart.Content = "Retake Quiz";
        }

        // ---------------------------------------------------------
        // ACTIVITY LOG (Activity Log tab) — reads from ActivityLogger
        // ---------------------------------------------------------

        // Clears the list box, then shows the most recent 'count' actions.
        // Shows a friendly message if nothing has been logged yet.
        private void DisplayRecentActivity(int count)
        {
            lstActivity.Items.Clear();

            List<ActivityEntry> recent = activityLogger.GetRecent(count);

            if (recent.Count == 0)
            {
                lstActivity.Items.Add("No activity recorded yet.");
                return;
            }

            foreach (ActivityEntry entry in recent)
            {
                lstActivity.Items.Add(entry.ToString());
            }
        }

        // Shows the 5 most recent actions.
        private void btnRefreshActivity_Click(object sender, RoutedEventArgs e)
        {
            DisplayRecentActivity(5);
        }

        // Shows the 10 most recent actions.
        private void btnShowMore_Click(object sender, RoutedEventArgs e)
        {
            DisplayRecentActivity(10);
        }

        // Records an action in the log, then refreshes the Activity Log tab
        // so the list is always up to date without pressing Refresh.
        private void LogAction(string description)
        {
            activityLogger.Log(description);
            DisplayRecentActivity(5);
        }

        // Adds a task straight from a chat message, e.g.
        // "Add a task to enable two-factor authentication" or
        // "Remind me to update my password tomorrow".
        private void AddTaskFromChat(string input)
        {
            string title = intentRecognizer.ExtractTaskTitle(input);
            DateTime? reminder = intentRecognizer.ExtractReminderDate(input);

            // Nothing left after removing the command words, e.g. the user only typed "add a task".
            if (string.IsNullOrWhiteSpace(title))
            {
                DisplayBotMessage("I recognised that you want to add a task, but I need the task details. " +
                                  "Try: Add a task to review my privacy settings.");
                return;
            }

            try
            {
                taskDb.AddTask(title, "Task created from chatbot conversation.", reminder);
            }
            catch (Exception ex)
            {
                DisplayBotMessage("Sorry, I couldn't save that task: " + ex.Message);
                return;
            }

            LogAction($"Task added through chatbot: {title}");
            if (reminder.HasValue)
            {
                LogAction($"Reminder set: {title} - {reminder.Value:dd MMM yyyy}");
            }

            RefreshTaskList();

            DisplayBotMessage(reminder.HasValue
                ? $"Done! I've added the task \"{title}\" with a reminder for {reminder.Value:dd MMM yyyy}."
                : $"Done! I've added the task \"{title}\".");
        }
    }
}
