using System;

namespace CybersecurityAwarenessBot
{
    public class Chatbot
    {
        //automatic properties
        public string BotName { get; set; }
        public string? UserName { get; set; }
        public string CurrentTopic { get; set; }
        public void Start()
        {
            //play voice greeting
            VoiceGreeting greeting = new VoiceGreeting();
            greeting.PlayGreeting();
            //Display ASII art
            AsciiArt art = new AsciiArt();
            art.DisplayLogo();
        }
        

        
        //method to run conversation
        public void StartConversation()
        { 
            //Ask user name
            Console.WriteLine("Please enter your name: ");
            string name = Console.ReadLine();
            //validate the name
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine(": I did not catch your name. Please enter your name so I can \r\npersonalise the conversation.");
                name = Console.ReadLine();

            }
            //display personalised welcome
            Console.WriteLine($"Welcome {name}!I'm your Cybersecurity Awareness Assistant. ");
            //Ask what the user wants to know

            ResponseHandler responseHandler = new ResponseHandler();
            string query = "";//start loop, read and validate the question 
            while (query != "exit")
            {
                Console.WriteLine("=====================================================================");
                Console.WriteLine("Enter a topic:");
                Console.WriteLine("1. Password");
                Console.WriteLine("2. Phishing");
                Console.WriteLine("3. Safe Browsing");
                Console.WriteLine("Type exit to leave the program.");
                Console.WriteLine("=====================================================================");

                query = Console.ReadLine();

                while (string.IsNullOrWhiteSpace(query))
                {
                    Console.WriteLine("Query cannot be empty. Please enter a topic.");
                    query = Console.ReadLine();
                }
                query = query.Trim().ToLower();

                //error-handling invalid numeric choice using TryParse
                if (int.TryParse(query, out int choice))

                {
                    // it WAS a valid whole number — now check if it's in range
                    if (choice == 1)
                    {
                        query = "password";
                    }
                    else if (choice == 2)
                    {
                        query = "phishing";
                    }
                    else if (choice == 3)
                    {
                        query = "safe browsing";
                    }
                    else
                    {
                        Console.WriteLine("Please enter a number between 1-3.");
                        continue; // skips the rest of this loop pass, goes straight back to asking
                    }
                }
                    if (query == "exit")
                    {
                        Console.WriteLine("Goodbye! Stay safe online.");
                        break;  // exits the while loop immediately
                    }
                    string response = responseHandler.GetResponse(query);
                    Console.WriteLine(response);
                }
            }
        }
    }