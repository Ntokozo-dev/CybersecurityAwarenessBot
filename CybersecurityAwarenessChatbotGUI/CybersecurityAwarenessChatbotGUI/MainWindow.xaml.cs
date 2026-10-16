using System.Windows;

namespace CybersecurityAwarenessChatbotGUI
{
    // "partial" because the other half of this class is auto-generated
    // from MainWindow.xaml. ": Window" means we inherit WPF's built-in
    // window behaviour (resizing, closing, rendering, etc.).
    public partial class MainWindow : Window
    {
        // Fields: these stay alive for as long as the window is open,
        // reused across every button click (not recreated each time).

        // Tracks which "stage" of the conversation we're in.
        private bool waitingForName = true;

        // Reused UserProfile object — stores and validates the user's name.
        private UserProfile user = new UserProfile();

        // Reused ResponseHandler object — looks up cybersecurity responses.
        private ResponseHandler responseHandler = new ResponseHandler();

        public MainWindow()
        {
            // Builds the window from the XAML above (auto-generated step).
            InitializeComponent();

            //subscribe: run MainWindow_Loaded once the window has finished loading.
            Loaded += MainWindow_Loaded;


        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)


        {

            // Play the voice greeting once, when the window first opens.
            VoiceGreeting greeting = new VoiceGreeting();
            greeting.PlayGreeting();

            AsciiArt art = new AsciiArt();
            rtbBanner.AppendText(art.GetLogo() + "\n");

            // Print the bot's opening lines into the conversation box.
            AppendToChat("Bot: Welcome to the Cybersecurity Awareness Bot!\n");
            AppendToChat("Bot: What is your name?\n");
        }


        // Runs every time the Send button is clicked.
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            // Grab whatever the user typed.
            string input = txtUserInput.Text;

            // Show the user's own message first, so both sides of the
            // conversation are visible.
            AppendToChat("You: " + input + "\n");

            if (waitingForName)
            {
                // Still waiting for a valid name.
                // SetName() both validates AND stores it; returns true/false.
                bool nameAccepted = user.SetName(input);

                if (nameAccepted)
                {
                    // Valid name: greet them and move to the next stage.
                    AppendToChat ($"Bot: Nice to meet you, {user.Name}! Ask me anything about cybersecurity.\n");
                    waitingForName = false;
                }
                else
                {
                    // Blank/invalid name: ask again, stay in this stage.
                    AppendToChat("Bot: I didn't catch that. Please enter your name.\n");
                }
            }
            else
            {
                // Already have a name — treat input as a cybersecurity topic.
                string response = responseHandler.GetResponse(input);
                AppendToChat("Bot: " + response + "\n");
            }

            // Clear the input box, ready for the next message.
            txtUserInput.Clear();
        }
    
        //Adds text to the chat window and keeps the newest message in view.
    private void AppendToChat(string message)
        {
            rtbChatHistory.AppendText(message);
            rtbChatHistory.ScrollToEnd();

        }
    }
}