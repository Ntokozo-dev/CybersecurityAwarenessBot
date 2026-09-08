using System;


namespace CybersecurityAwarenessBot
{
    public class ResponseHandler
    {
        public string GetResponse(string userInput)
        {
            //Convert the input to a consistent format.
            userInput = userInput.ToLower().Trim();
            //check for recognised words or questions and return the appropriate response.
            if (userInput.Contains("password"))
            {
                return "Use a strong, unique password and avoid using the same password across different accounts.";
            }
            else if (userInput.Contains("phishing"))
            {
                return "Be careful of suspicious emails, messages, and links asking for personal information.";
            }
            else if (userInput.Contains("safe browsing"))
            {
                return "Only visit trusted websites, check that the URL is correct, and avoid clicking suspicious links.";
            }
            //otherwise return a default response
            else
            {
                return "I'm not sure I understand. Please choose a topic such as password, phishing, or safe browsing.";
            }
        }
    }
}
