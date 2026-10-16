using System.Collections.Generic;

namespace CybersecurityAwarenessChatbotGUI
{
   
    public static class TextHelper
    {
        public static string Normalise(string text)
        {
            //trims spaces, lower-cases it  and turns the
            // curly apostrophe that phone keyboards type into a normal one.
            return text.Trim().ToLowerInvariant().Replace('\u2019', '\'');
        }

        // True if the text contains ANY of the given words/phrases.Produces appropriate response.
        // Using Contains means "tell me about password safety" still matches "password".
        public static bool ContainsAny(string text, IEnumerable<string> phrases)
        {
            foreach (string phrase in phrases)
            {
                if (text.Contains(phrase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}