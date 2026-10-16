using System;
using System.Collections.Generic;

namespace CybersecurityAwarenessChatbotGUI
{
    ///The moods the bot can recognise.
    public enum Sentiment
    {
        Neutral,
        Worried,
        Curious,
        Frustrated,
        Unsure
    }

    public class SentimentDetector
    {
        // One Random object, created once and reused (a new one per call can repeat values).
        private readonly Random random = new Random();

        // Mood -> words that suggest it. The order matters: the first mood that matches wins.
        // Word stems such as "frustrat" also catch "frustrated" and "frustrating".
        private readonly Dictionary<Sentiment, List<string>> sentimentWords =
            new Dictionary<Sentiment, List<string>>
            {
                [Sentiment.Frustrated] = new List<string>
                    { "frustrat", "annoy", "angry", "fed up", "irritat", "sick of" },

                [Sentiment.Worried] = new List<string>
                    { "worried", "worry", "scared", "afraid", "anxious", "nervous",
                      "terrified", "concerned", "panic", "fear" },

                [Sentiment.Unsure] = new List<string>
                    { "confus", "overwhelm", "don't understand", "do not understand",
                      "dont understand", "not sure", "unsure", "feel lost", "too much", "stuck" },

                [Sentiment.Curious] = new List<string>
                    { "curious", "wondering", "i wonder", "want to know", "want to learn", "keen to" }
            };

        // Several openings per mood so the bot does not sound repetitive.
        // {topic} and {name} are placeholders filled in by GetOpening.
        // Every {name} is written as ", {name}" so it can be removed cleanly if we don't know the name.
        private readonly Dictionary<Sentiment, List<string>> openings =
            new Dictionary<Sentiment, List<string>>
            {
                [Sentiment.Worried] = new List<string>
                {
                    "It's completely understandable to feel worried about {topic}, {name}. Let me share a tip to help you stay safe.",
                    "I hear you, {name}, and your concern is valid. Here's something that can help you feel more in control.",
                    "Feeling anxious about {topic} is completely normal, {name}. The good news is that a few simple habits go a long way."
                },
                [Sentiment.Frustrated] = new List<string>
                {
                    "I'm sorry this is frustrating, {name}. Let's take it one small step at a time.",
                    "That sounds annoying, {name}, and it's okay to feel that way. Here's a simple tip on {topic} to make things easier.",
                    "Cybersecurity can be frustrating, {name}, but you're not alone. Here's something practical."
                },
                [Sentiment.Unsure] = new List<string>
                {
                    "No worries, {name}, there's no need to have it all figured out. Let's keep it simple.",
                    "It's okay to feel unsure about {topic}, {name}. Here's a clear place to start.",
                    "That's a lot to take in, {name}, so let's go step by step."
                },
                [Sentiment.Curious] = new List<string>
                {
                    "Great question, {name}! Curiosity is one of the best security habits.",
                    "I love your curiosity about {topic}, {name}. Here's something worth knowing.",
                    "Good thinking, {name}! Asking questions is how you stay one step ahead."
                }
            };

        // Returns the mood found in the (already normalised) input, or Neutral if none.
        public Sentiment Detect(string input)
        {
            foreach (KeyValuePair<Sentiment, List<string>> entry in sentimentWords)
            {
                if (TextHelper.ContainsAny(input, entry.Value))
                {
                    return entry.Key;
                }
            }
            return Sentiment.Neutral;
        }

        // Builds the empathetic opening sentence for a mood.
        public string GetOpening(Sentiment sentiment, string topic, string userName)
        {
            if (!openings.ContainsKey(sentiment))
            {
                return "";
            }

            List<string> options = openings[sentiment];
            string template = options[random.Next(options.Count)];

            string topicLabel = string.IsNullOrWhiteSpace(topic) ? "online safety" : topic;
            string text = template.Replace("{topic}", topicLabel);

            // Use the name if we have it, otherwise drop ", {name}" so the sentence still reads well.
            return string.IsNullOrWhiteSpace(userName)
                ? text.Replace(", {name}", "")
                : text.Replace("{name}", userName);
        }
    }
}