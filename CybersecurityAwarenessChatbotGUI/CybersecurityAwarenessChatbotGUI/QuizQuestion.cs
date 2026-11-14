using System;
using System.Collections.Generic;


namespace CybersecurityAwarenessChatbotGUI
{
    // Represents one quiz question — works for both multiple-choice and true/false,
    // since true/false is really just a multiple-choice question with two options.
    public class QuizQuestion
    {
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }  // e.g. ["A", "B", "C", "D"] or ["True", "False"]
        public string CorrectAnswer { get; set; }
    }
}