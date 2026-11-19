using System;


namespace CybersecurityAwarenessChatbotGUI
{
    public class ActivityEntry
    {
        public DateTime Time { get; set; }
        public string Description { get; set; } = "";

        public override string ToString()
        {
            return $"{Time:HH:mm} - {Description}";
        }
    }
}
