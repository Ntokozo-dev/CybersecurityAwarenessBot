using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections.Generic;

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

        // Colours for the chat: the user's messages are white, the bot's are light green.
        private readonly Brush userBrush = Brushes.White;
        private readonly Brush botBrush = Brushes.LightGreen;

        public MainWindow()
        {
            // Builds the window from the XAML (auto-generated step).
            InitializeComponent();

            // --- TEMPORARY TEST CODE ---
            TaskDatabase db = new TaskDatabase();
            List<CyberTask> allTasks = db.GetAllTasks();

            string result = $"Found {allTasks.Count} task(s):\n\n";
            foreach (CyberTask task in allTasks)
            {
                result += $"ID: {task.TaskId}, Title: {task.Title}, Completed: {task.IsCompleted}\n";
            }

            MessageBox.Show(result);
            // --- END TEMPORARY TEST CODE ---

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
                // Hand the message to the chatbot. The reply comes back through the delegate.
                responseHandler.ProcessInput(input, user.Name);
            }

            // Clear the box and keep the cursor there, ready for the next message.
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
    }
}