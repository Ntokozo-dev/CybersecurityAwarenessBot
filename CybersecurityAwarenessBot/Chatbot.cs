using System;

namespace CybersecurityAwarenessBot
{ 
    
    public class Chatbot
    {
        public string? UserName { get; set; }
        public string CurrentTopic { get; set; }
        public string BotName { get; set; }
        public void Start()
        {
            //play voice greeting
            VoiceGreeting greeting = new VoiceGreeting();
            greeting.PlayGreeting();
            //Display ASII art
            AsciiArt art = new AsciiArt();
            art.DisplayLogo();

            StartConversation();
        }
        //method to run conversation
        public void StartConversation()
        {
            //Ask user name
            UserProfile User = new UserProfile();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("What is your name? ");
            Console.ForegroundColor = ConsoleColor.White;
            string name = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Green;
            //validate the name
            while (!User.SetName(name))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(": I did not catch your name. Please enter your name so I can \r\npersonalise the conversation.");
                Console.ForegroundColor = ConsoleColor.White;
                name = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Green;

            }
            UserName = User.Name;
            //display personalised welcome
            Console.WriteLine($"Welcome {User.Name}!I'm your Cybersecurity Awareness Assistant. ");
        
            //response system

            ResponseHandler responseHandler = new ResponseHandler();
            string query = "";//start loop, read and validate the question 
            while (query != "exit")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=====================================================================");
                Console.WriteLine("Enter a topic:");
                Console.WriteLine("1. Password");
                Console.WriteLine("2. Phishing");
                Console.WriteLine("3. Safe Browsing");
                Console.WriteLine("Type exit to leave the program.");
                Console.WriteLine("=====================================================================");

                Console.ForegroundColor = ConsoleColor.White;
                query = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Green;

                while (string.IsNullOrWhiteSpace(query))
                {
                    Console.WriteLine("Query cannot be empty. Please enter a topic.");
                    query = Console.ReadLine();
                }
                query = query.Trim().ToLower();

                    if (query == "exit")
                    {
                        Console.WriteLine("Goodbye! Stay safe online.");
                        break;  // exits the while loop immediately
                    }
                    string response = responseHandler.GetResponse(query);
                    Console.WriteLine(response);
            }
            Console.ResetColor();
        }
       
        }
    }