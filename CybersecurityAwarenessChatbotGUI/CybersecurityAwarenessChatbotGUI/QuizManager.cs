using System;
using System.Collections.Generic;


namespace CybersecurityAwarenessChatbotGUI
{
    public class QuizManager
    {
        private List<QuizQuestion> questions;
        private int currentIndex;
        private int score;

        public QuizManager()
        {
            currentIndex = 0;
            score = 0;
            questions = BuildQuestionBank();
        }

        // Returns the current question, or null if the quiz is finished.
        public QuizQuestion GetCurrentQuestion()
        {
            if (currentIndex < questions.Count)
            {
                return questions[currentIndex];
            }
            return null;
        }
        // Checks the given answer against the current question, updates the score,
        // moves to the next question, and returns the result for display.
        public QuizAnswerResult SubmitAnswer(string answer)
        {
            QuizQuestion current = questions[currentIndex];
            bool correct = current.CorrectAnswer.Equals(answer, System.StringComparison.OrdinalIgnoreCase);

            if (correct)
            {
                score++;
            }

            QuizAnswerResult result = new QuizAnswerResult
            {
                IsCorrect = correct,
                CorrectAnswer = current.CorrectAnswer
            };

            currentIndex++;
            return result;
        }

        public bool IsFinished()
        {
            return currentIndex >= questions.Count;
        }
        public int GetScore()
        {
            return score;
        }

        public int GetTotalQuestions()
        {
            return questions.Count;
        }

        // The question bank: 12 questions, mixing multiple-choice and true/false.
        private List<QuizQuestion> BuildQuestionBank()
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion { QuestionText = "What should you do if you receive an email asking for your password?", Options = new List<string> { "Reply with it", "Delete or report it", "Forward it to a friend", "Ignore it silently" }, CorrectAnswer = "Delete or report it" },
                new QuizQuestion { QuestionText = "Using the same password across multiple accounts is safe.", Options = new List<string> { "True", "False" }, CorrectAnswer = "False" },
                new QuizQuestion { QuestionText = "What is phishing?", Options = new List<string> { "A type of computer virus", "A fraudulent attempt to obtain personal information", "A firewall setting", "A type of encryption" }, CorrectAnswer = "A fraudulent attempt to obtain personal information" },
                new QuizQuestion { QuestionText = "Multi-factor authentication adds an extra layer of security.", Options = new List<string> { "True", "False" }, CorrectAnswer = "True" },
                new QuizQuestion { QuestionText = "Which of these is a strong password?", Options = new List<string> { "123456", "password", "Tr0ub4dor&3xplor3", "yourname" }, CorrectAnswer = "Tr0ub4dor&3xplor3" },
                new QuizQuestion { QuestionText = "Public Wi-Fi networks are always safe for online banking.", Options = new List<string> { "True", "False" }, CorrectAnswer = "False" },
                new QuizQuestion { QuestionText = "What does VPN stand for?", Options = new List<string> { "Virtual Private Network", "Verified Public Node", "Virus Protection Network", "Very Personal Network" }, CorrectAnswer = "Virtual Private Network" },
                new QuizQuestion { QuestionText = "You should click links in unexpected emails to verify if they are safe.", Options = new List<string> { "True", "False" }, CorrectAnswer = "False" },
                new QuizQuestion { QuestionText = "What is a common sign of a phishing email?", Options = new List<string> { "Urgent language demanding immediate action", "A company logo", "A short message", "Correct spelling" }, CorrectAnswer = "Urgent language demanding immediate action" },
                new QuizQuestion { QuestionText = "Regularly updating software helps protect against security vulnerabilities.", Options = new List<string> { "True", "False" }, CorrectAnswer = "True" },
                new QuizQuestion { QuestionText = "What should you check before entering personal details on a website?", Options = new List<string> { "The website's colour scheme", "That the URL starts with https and looks correct", "The number of ads on the page", "The page loading speed" }, CorrectAnswer = "That the URL starts with https and looks correct" },
                new QuizQuestion { QuestionText = "Sharing your location on social media always improves your privacy.", Options = new List<string> { "True", "False" }, CorrectAnswer = "False" }
            };
        }
    }
}