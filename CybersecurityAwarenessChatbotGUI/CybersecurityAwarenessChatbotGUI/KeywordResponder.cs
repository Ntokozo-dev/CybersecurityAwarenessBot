using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityAwarenessChatbotGUI
{
    /// Holds all the bot's cybersecurity knowledge in Dictionaries and Lists
    public class KeywordResponder
    {
        // One Random object, created once and reused. Makes responses more varied and less repetitive.
        private readonly Random random = new Random();

        //prevent consecutive repeated tips for the same topic.
        private readonly Dictionary<string, int> lastIndexUsed = new Dictionary<string, int>();

        // dictionary we use use to store keywords
        private readonly Dictionary<string, List<string>> topicKeywords =
            new Dictionary<string, List<string>>
            {
                ["passwords"] = new List<string> { "password", "passphrase", "passcode" },
                ["phishing"] = new List<string> { "phish", "suspicious email", "fake email", "spam email" },
                ["scams"] = new List<string> { "scam", "fraud", "swindle", "con artist" },
                ["privacy"] = new List<string> { "privacy", "personal data", "personal information", "data protection", "tracking" },
                ["safe browsing"] = new List<string> { "safe browsing", "browsing", "browser", "website", "https" },
                ["two-factor authentication"] = new List<string> { "2fa", "two-factor", "two factor", "multi-factor", "multifactor", "mfa", "authenticator", "verification code" },
                ["malware"] = new List<string> { "malware", "virus", "ransomware", "spyware", "antivirus", "trojan" }
            };

        
        // Several tips per topic. GetRandomResponse picks one, so asking twice gives variety.
        private readonly Dictionary<string, List<string>> topicResponses =
            new Dictionary<string, List<string>>
            {
                ["passwords"] = new List<string>
                {
                    "Use a strong, unique password for every account so one leak can't unlock everything.",
                    "Aim for long passphrases (four or more random words) rather than short, complicated passwords.",
                    "Avoid personal details like birthdays or pet names in your passwords. They're easy to guess.",
                    "Consider a trusted password manager so you only have to remember one strong master password."
                },
                ["phishing"] = new List<string>
                {
                    "Be cautious of emails or messages asking for personal information. Scammers often pose as trusted organisations.",
                    "Check the sender's address carefully. Phishing emails often use addresses that look almost right.",
                    "Hover over links before clicking, and never open attachments you weren't expecting.",
                    "Urgent wording like \"act now or your account will be closed\" is a classic phishing trick. Slow down and verify through the official website."
                },
                ["scams"] = new List<string>
                {
                    "If an offer sounds too good to be true, it probably is. Take a moment before sending money or details.",
                    "Never share one-time PINs or passwords with anyone, even if they claim to be from your bank.",
                    "Scammers create urgency or fear. Pause, then contact the organisation using an official number you look up yourself.",
                    "Search for the message's wording online before responding. Many scams have already been reported by others."
                },
                ["privacy"] = new List<string>
                {
                    "Review the privacy settings on your social media and app accounts regularly.",
                    "Only share the personal information that's truly needed, and check app permissions for things like location and microphone.",
                    "Use a separate email address for sign-ups you don't fully trust to limit tracking and spam.",
                    "Be careful what you post publicly. Details like your school, workplace or routine can be used against you."
                },
                ["safe browsing"] = new List<string>
                {
                    "Only visit trusted websites, check that the address is correct, and avoid clicking suspicious links.",
                    "Look for \"https\" and a padlock before entering personal information, but remember that alone doesn't guarantee a site is safe.",
                    "Keep your browser and extensions updated, and remove extensions you don't use.",
                    "Download software only from official sources, and be wary of pop-ups claiming your device is infected."
                },
                ["two-factor authentication"] = new List<string>
                {
                    "Turn on two-factor authentication wherever it's offered. A stolen password alone won't be enough to get in.",
                    "Authenticator apps are generally safer than text-message codes for two-factor authentication.",
                    "Keep backup codes for important accounts somewhere safe in case you lose your phone.",
                    "Never share a verification code with anyone. Real companies will never ask you for it."
                },
                ["malware"] = new List<string>
                {
                    "Keep your operating system and apps updated. Many attacks rely on security holes that already have fixes.",
                    "Install reputable antivirus software and run regular scans.",
                    "Don't plug in unknown USB drives or open attachments you weren't expecting.",
                    "Back up your important files regularly so ransomware can't hold them hostage."
                },
                // Used when the user sounds worried/frustrated but has not named a topic.
                ["general"] = new List<string>
                {
                    "A great starting point: use strong, unique passwords and turn on two-factor authentication.",
                    "Stay sceptical of unexpected messages, especially ones that create urgency.",
                    "Keep your devices and apps updated. Updates often contain important security fixes."
                }
            };

        // Reminder lines used to personalise replies when the user has a favourite topic.
        private readonly Dictionary<string, string> personalNotes =
            new Dictionary<string, string>
            {
                ["passwords"] = "As someone interested in passwords, you might want to check whether your most important accounts use a unique password.",
                ["phishing"] = "As someone interested in phishing, you could try spotting the warning signs in the next suspicious email you receive.",
                ["scams"] = "As someone interested in scams, you might want to tell a friend or relative about the latest tricks to watch out for.",
                ["privacy"] = "As someone interested in privacy, you might want to review the security settings on your accounts.",
                ["safe browsing"] = "As someone interested in safe browsing, you might want to check that your browser is fully up to date.",
                ["two-factor authentication"] = "As someone interested in two-factor authentication, you might want to switch it on for your email account first.",
                ["malware"] = "As someone interested in malware, you might want to run a full security scan this week."
            };

        //Small talk (basic responses carried over from Part 1)
        private readonly Dictionary<string, List<string>> smallTalkPhrases =
            new Dictionary<string, List<string>>
            {
                ["wellbeing"] = new List<string> { "how are you", "how's it going", "how are things" },
                ["purpose"] = new List<string> { "purpose", "why were you", "why do you exist" },
                ["help"] = new List<string> { "what can i ask", "what can you do", "help me", "need help" },
                ["identity"] = new List<string> { "who are you", "what are you", "your name" },
                ["greeting"] = new List<string> { "hello", "hey there", "hi there", "good morning", "good afternoon", "good evening", "howdy" },
                ["thanks"] = new List<string> { "thank", "cheers" },
                ["goodbye"] = new List<string> { "bye", "see you", "see ya" }
            };

        // {topics} is replaced with the real list of topics.
        private readonly Dictionary<string, List<string>> smallTalkReplies =
            new Dictionary<string, List<string>>
            {
                ["wellbeing"] = new List<string>
                {
                    "I'm doing well, thank you for asking! I hope you are well too.",
                    "I'm great, thanks! Ready to help you stay safe online."
                },
                ["purpose"] = new List<string>
                {
                    "I was created to help you build safe digital habits.",
                    "My purpose is to teach cybersecurity awareness, one tip at a time."
                },
                ["help"] = new List<string>
                {
                    "You can ask me about {topics}. You can also ask how I am or what my purpose is.",
                    "Try asking about {topics}, or say \"tell me more\" to keep a topic going."
                },
                ["identity"] = new List<string>
                {
                    "I'm the Cybersecurity Awareness Bot, here to help you with safe digital practices."
                },
                ["greeting"] = new List<string>
                {
                    "Hello! Ask me about {topics} and I'll share a tip.",
                    "Hi there! What would you like to know about staying safe online?"
                },
                ["thanks"] = new List<string>
                {
                    "You're welcome! Ask me anything else about staying safe online.",
                    "Happy to help! Stay safe out there."
                },
                ["goodbye"] = new List<string>
                {
                    "Goodbye! Stay safe online.",
                    "See you next time. Remember to think before you click!"
                }
            };

        //  Follow-ups (for a more natural conversation flow)
        private readonly List<string> followUpPhrases = new List<string>
        {
            "tell me more", "more info", "more details", "explain more", "explain further",
            "explain that", "another tip", "another one", "give me another", "one more",
            "what else", "continue", "go on", "keep going", "elaborate", "next tip",
            "don't understand", "do not understand", "dont understand", "confused", "unclear"
        };

        private readonly List<string> followUpLeadIns = new List<string>
        {
            "Here's another tip on {topic}:",
            "Sure! More on {topic}:",
            "Happy to expand on {topic}:",
            "Here's something else about {topic}:"
        };

         // Returns the topic found in the input, or "" if there is none.Error handling
        public string DetectTopic(string input)
        {
            foreach (KeyValuePair<string, List<string>> topic in topicKeywords)
            {
                if (TextHelper.ContainsAny(input, topic.Value))
                {
                    return topic.Key;
                }
            }
            return "";
        }

        // Picks one of the stored tips for the topic at random (never the same one twice in a row).
        public string GetRandomResponse(string topic)
        {
            List<string> options = topicResponses.ContainsKey(topic)
                ? topicResponses[topic]
                : topicResponses["general"];
            return PickRandom(topic, options);
        }

        // Handles everyday questions such as "how are you" or "what is your purpose".
        public bool TryGetSmallTalk(string input, out string reply)
        {
            foreach (KeyValuePair<string, List<string>> intent in smallTalkPhrases)
            {
                if (TextHelper.ContainsAny(input, intent.Value))
                {
                    reply = PickRandom(intent.Key, smallTalkReplies[intent.Key])
                        .Replace("{topics}", TopicListText());
                    return true;
                }
            }
            reply = "";
            return false;
        }

        // True for phrases like "tell me more" or "give me another tip".
        public bool IsFollowUp(string input)
        {
            return TextHelper.ContainsAny(input, followUpPhrases);
        }

        public string GetFollowUpLeadIn(string topic)
        {
            string template = followUpLeadIns[random.Next(followUpLeadIns.Count)];
            return template.Replace("{topic}", topic);
        }

        // The personalised reminder for a favourite topic ("" if there isn't one).
        public string GetPersonalNote(string topic)
        {
            return personalNotes.TryGetValue(topic, out string note) ? note : "";
        }

        // Friendly reply for input the bot does not recognise (error handling).
        public string GetFallbackMessage()
        {
            return "I'm not sure I understand. Can you try rephrasing, or ask me about "
                   + TopicListText() + "?";
        }

        // Builds "passwords, phishing, scams, ... or malware" from the dictionary,
        // so the list is never out of date when a topic is added.
        public string TopicListText()
        {
            List<string> names = topicKeywords.Keys.ToList();
            if (names.Count == 1)
            {
                return names[0];
            }
            return string.Join(", ", names.Take(names.Count - 1)) + " or " + names[names.Count - 1];
        }

        // Random pick that avoids repeating the previous choice for the same key.
        private string PickRandom(string key, List<string> options)
        {
            int index = random.Next(options.Count);

            if (options.Count > 1 && lastIndexUsed.TryGetValue(key, out int last))
            {
                // Choose any index except 'last'.
                index = (last + 1 + random.Next(options.Count - 1)) % options.Count;
            }

            lastIndexUsed[key] = index;
            return options[index];
        }
    }
}