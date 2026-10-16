using System;

namespace CybersecurityAwarenessChatbotGUI
{
    internal class UserProfile
    {
        // automatic property to store the user's name
        public string Name { get; set; } = "";

        public UserProfile()
        {
        }

        //returns true once a real(not empty) name has been set
        public bool HasName()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }
        //sets the name only if it passes basic validation
        public bool SetName(string inputName)
        {
             if (string.IsNullOrWhiteSpace(inputName))
            {
                return false;
            }
            Name = inputName.Trim();
            return true;
        }
        
    }
}
