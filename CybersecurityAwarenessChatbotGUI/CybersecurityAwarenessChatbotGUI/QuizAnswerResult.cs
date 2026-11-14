using System;
using System.Collections.Generic;

namespace CybersecurityAwarenessChatbotGUI
{
    // Represents the outcome of answering one question:
    // whether it was correct, and what the correct answer actually was
    // (useful for showing feedback even when the user got it wrong).
    public class QuizAnswerResult
    {
        public bool IsCorrect { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
