using System.Text;

namespace CybersecurityAwarenessChatbotGUI
{
    public class AsciiArt
    {
        public string GetLogo()
        {
            StringBuilder logo = new StringBuilder();
            logo.AppendLine("============================================");
            logo.AppendLine("     CYBERSECURITY AWARENESS BOT            ");
            logo.AppendLine("============================================");
            logo.AppendLine("                   .-----.                  ");
            logo.AppendLine("                  /  ___  \\                ");
            logo.AppendLine("                 |  /   \\  |                ");
            logo.AppendLine("                 | |     | |                ");
            logo.AppendLine("                 |  \\___/  |                ");
            logo.AppendLine("                 |---------|                ");
            logo.AppendLine("                 |  #####  |                ");
            logo.AppendLine("                 |  #####  |                ");
            logo.AppendLine("                 '---------'                ");
            logo.AppendLine("       [ ENCRYPTED ]  [ FIREWALL ]  [ SAFE ]");
            logo.AppendLine("============================================");
            return logo.ToString();
        }
    }
}