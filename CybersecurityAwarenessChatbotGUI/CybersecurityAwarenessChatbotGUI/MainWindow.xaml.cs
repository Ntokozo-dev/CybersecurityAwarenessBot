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
                        break;

                    case UserIntent.ShowTasks:
                        DisplayBotMessage("Here are your tasks — switching to the Tasks tab.");
                        TasksTabItem.IsSelected = true;
                        RefreshTaskList();
                        break;

                    case UserIntent.AddTask:
                        DisplayBotMessage("Let's add a task — switching to the Tasks tab so you can fill in the details.");
                        TasksTabItem.IsSelected = true;
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

            btnQuizSubmit.IsEnabled = false;
            btnQuizNext.IsEnabled = false;
            btnQuizStart.IsEnabled = true; // allow retaking the quiz
            btnQuizStart.Content = "Retake Quiz";
        }
    }
}
  
