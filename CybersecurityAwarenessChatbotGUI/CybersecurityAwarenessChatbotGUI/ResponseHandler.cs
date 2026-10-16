using System.Collections.Generic;

namespace CybersecurityAwarenessChatbotGUI
{
   
    public delegate void BotMessageHandler(string message);

    
    public class ResponseHandler
    {
        private readonly KeywordResponder keywordResponder = new KeywordResponder();
        private readonly SentimentDetector sentimentDetector = new SentimentDetector();
        private readonly MemoryStore memory = new MemoryStore();

        // Phrases that mean "what do you remember about me?"
        private readonly List<string> memoryPhrases = new List<string>
        {
            "what do you remember", "what do you know about me", "do you remember",
            "what have i told you", "my interests"
        };

        // Phrases that mean the user is telling us about an interest (stored as favourite topic).
        private readonly List<string> interestPhrases = new List<string>
        {
            "interested in", "favourite topic", "favorite topic", "my favourite", "my favorite",
            "i like", "i love", "i care about", "i enjoy"
        };

        // The window plugs a method into this property. When a reply is ready we "invoke" it.
        public BotMessageHandler? MessageReady { get; set; }

        // List of topics, used by the window for its welcome message.
        public string GetTopicList()
        {
            return keywordResponder.TopicListText();
        }

        // Entry point called by the window when the user clicks Send.
        public void ProcessInput(string userInput, string userName)
        {
            string reply = BuildReply(userInput, userName);

            // "?." means: only call it if a method has been plugged in (not null).
            MessageReady?.Invoke(reply);
        }

        // Works out the reply text (follows the numbered order in the class comment).
        private string BuildReply(string userInput, string userName)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "Please type a question or cybersecurity topic first.";
            }

            string input = TextHelper.Normalise(userInput);

            if (TextHelper.ContainsAny(input, memoryPhrases))
            {
                return memory.Recall(userName);
            }

            Sentiment sentiment = sentimentDetector.Detect(input);
            string topic = keywordResponder.DetectTopic(input);

            // A cybersecurity topic was found: this always takes priority.
            if (topic != "")
            {
                return BuildTopicReply(topic, sentiment, userName, input);
            }

            // No new topic: is this a follow-up such as "tell me more"?
            if (keywordResponder.IsFollowUp(input))
            {
                return BuildFollowUpReply(sentiment, userName);
            }

            // Everyday questions from Part 1 ("how are you", "what is your purpose").
            if (keywordResponder.TryGetSmallTalk(input, out string smallTalkReply))
            {
                return smallTalkReply;
            }

            // A feeling but no topic ("I'm worried"): support the user AND give a tip.
            if (sentiment != Sentiment.Neutral)
            {
                return BuildSentimentOnlyReply(sentiment, userName);
            }

            // Nothing recognised: friendly fallback, never an error.
            return keywordResponder.GetFallbackMessage();
        }

        // Reply when a cybersecurity topic was recognised.
        private string BuildTopicReply(string topic, Sentiment sentiment, string userName, string input)
        {
            memory.RecordTopic(topic);
            string tip = keywordResponder.GetRandomResponse(topic);

            // "I'm interested in privacy" -> remember it as the favourite topic.
            if (TextHelper.ContainsAny(input, interestPhrases))
            {
                memory.FavouriteTopic = topic;
                return $"Great! I'll remember that you're interested in {topic}. Here's a first tip: {tip}";
            }

            string reply = tip;
            if (sentiment != Sentiment.Neutral)
            {
                // Empathy first, then the tip in the SAME reply (required by the brief).
                reply = sentimentDetector.GetOpening(sentiment, topic, userName) + " " + tip;
            }

            return AddPersonalNote(reply, topic);
        }

        // Reply for "tell me more", "give me another tip", "I don't understand", etc.
        private string BuildFollowUpReply(Sentiment sentiment, string userName)
        {
            string topic = memory.CurrentTopic;

            // Nothing to continue yet.
            if (topic == "")
            {
                string prompt = "Happy to keep going! Which topic would you like to explore? You can ask about "
                                + keywordResponder.TopicListText() + ".";

                return sentiment == Sentiment.Neutral
                    ? prompt
                    : sentimentDetector.GetOpening(sentiment, "", userName) + " " + prompt;
            }

            // Continue the SAME topic with a different tip.
            string tip = keywordResponder.GetRandomResponse(topic);
            string opening = sentiment == Sentiment.Neutral
                ? keywordResponder.GetFollowUpLeadIn(topic)
                : sentimentDetector.GetOpening(sentiment, topic, userName);

            return AddPersonalNote(opening + " " + tip, topic);
        }

        // Reply when the user expresses a feeling without naming a topic.
        private string BuildSentimentOnlyReply(Sentiment sentiment, string userName)
        {
            string topic = memory.CurrentTopic;

            // Use the current topic if there is one, otherwise a general tip.
            string tip = keywordResponder.GetRandomResponse(topic == "" ? "general" : topic);
            string opening = sentimentDetector.GetOpening(sentiment, topic, userName);

            return opening + " " + tip + " You can also ask me about " + keywordResponder.TopicListText() + ".";
        }

        // Adds the "as someone interested in..." line when it is time to (see MemoryStore).
        private string AddPersonalNote(string reply, string topic)
        {
            if (memory.ShouldAddPersonalNote(topic))
            {
                string note = keywordResponder.GetPersonalNote(memory.FavouriteTopic);
                if (note != "")
                {
                    reply += " " + note;
                }
            }
            return reply;
        }
    }
}