using System.Collections.Generic;

namespace CybersecurityAwarenessChatbotGUI
{
    // The bot's "memory". It stores things the user has told us so the bot can
    public class MemoryStore
    {
        //properties for storing and retrieving (favorite and current topic)
        public string FavouriteTopic { get; set; } = "";
        public string CurrentTopic { get; set; } = "";

        // Every topic the user has asked about, without duplicates.
        public List<string> TopicsDiscussed { get; } = new List<string>();

        // Used so the personal reminder appears every few replies instead of every time.
        private int repliesSincePersonalNote = 0;

        // Called whenever a topic is detected in the user's message.
        public void RecordTopic(string topic)
        {
            CurrentTopic = topic;
            if (!TopicsDiscussed.Contains(topic))
            {
                TopicsDiscussed.Add(topic);
            }
        }

        // Decides whether to add a "since you're interested in X..." line to a reply.
        // It only happens when a favourite topic exists, the reply is about a DIFFERENT
        // topic, and at least two replies have passed since the last reminder.
        public bool ShouldAddPersonalNote(string topic)
        {
            if (FavouriteTopic == "" || topic == FavouriteTopic)
            {
                return false;
            }

            repliesSincePersonalNote++;
            if (repliesSincePersonalNote >= 2)
            {
                repliesSincePersonalNote = 0;
                return true;
            }
            return false;
        }

        // Summary of everything remembered (used when the user asks "what do you remember?").
        public string Recall(string userName)
        {
            List<string> parts = new List<string>();

            parts.Add(string.IsNullOrWhiteSpace(userName)
                ? "I don't know your name yet."
                : $"Your name is {userName}.");

            parts.Add(FavouriteTopic == ""
                ? "You haven't told me a favourite topic yet."
                : $"You're interested in {FavouriteTopic}.");

            parts.Add(TopicsDiscussed.Count == 0
                ? "We haven't covered any topics yet."
                : "So far we've talked about " + string.Join(", ", TopicsDiscussed) + ".");

            return string.Join(" ", parts);
        }
    }
}