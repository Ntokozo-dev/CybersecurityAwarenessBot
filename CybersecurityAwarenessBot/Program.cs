using System;

namespace CybersecurityAwarenessBot;

public class Program
{
    static void Main(String[] args)
    {
        Chatbot bot = new Chatbot();
        bot.Start();
        bot.StartConversation();
    }
}