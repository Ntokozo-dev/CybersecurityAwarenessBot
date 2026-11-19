using System;

namespace CybersecurityAwarenessChatbotGUI
{
    public class ActivityLogger
    {
        private readonly List<ActivityEntry> entries =
new List<ActivityEntry>();
        public void Log(string description)
        {
            entries.Add(new ActivityEntry
            {
                Time = DateTime.Now,
                Description = description
            });
        }
public List<ActivityEntry> GetRecent(int count)
        { 
return entries
.OrderByDescending(x => x.Time)
.Take(count)
.ToList();

    }
  }
}

