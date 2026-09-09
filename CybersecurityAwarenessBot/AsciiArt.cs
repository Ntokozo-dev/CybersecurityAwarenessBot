using System;


namespace CybersecurityAwarenessBot
{
    public class AsciiArt
    {
        public void DisplayLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("============================================");
            Console.WriteLine("     CYBERSECURITY AWARENESS BOT            ");
            Console.WriteLine("============================================");
            Console.WriteLine("                   .-----.                  ");
            Console.WriteLine("                  /  ___  \\                ");
            Console.WriteLine("                 |  /   \\  |                ");
            Console.WriteLine("                 | |     | |                ");
            Console.WriteLine("                 |  \\___/  |                ");
            Console.WriteLine("                 |---------|                ");
            Console.WriteLine("                 |  #####  |                ");
            Console.WriteLine("                 |  #####  |                ");
            Console.WriteLine("                 '---------'                ");
            Console.WriteLine("       [ ENCRYPTED ]  [ FIREWALL ]  [ SAFE ]");
            Console.WriteLine("============================================");

            Console.ResetColor();
        }
    }
}