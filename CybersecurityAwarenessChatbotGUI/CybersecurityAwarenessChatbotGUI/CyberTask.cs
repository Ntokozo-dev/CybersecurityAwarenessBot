using System;

namespace CybersecurityAwarenessChatbotGUI
{
    // Represents one row from the "tasks" table as a C# object.
    // Each property here matches a column in the database table,
    // so we can easily map between SQL rows and this class.
    public class CyberTask
    {
        public int TaskId { get; set; }          // matches TaskId column (auto-increment primary key)
        public string Title { get; set; }         // matches Title column (required)
        public string Description { get; set; }   // matches Description column (optional)
        public DateTime? Reminder { get; set; }    // matches Reminder column; nullable since it's optional
        public bool IsCompleted { get; set; }      // matches IsCompleted column (true/false)
    }
}