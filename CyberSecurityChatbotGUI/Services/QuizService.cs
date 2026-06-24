using System;
using System.Collections.Generic;
using CyberSecurityChatbotGUI.Models;

namespace CyberSecurityChatbotGUI.Services
{
    public class QuizService
    {
        private List<QuizQuestion> _questions;
        private int _currentIndex;
        private int _score;
        private Random _random = new Random();

        public int TotalQuestions => _questions.Count;
        public int CurrentQuestionNumber => _currentIndex + 1;
        public int Score => _score;
        public bool IsFinished => _currentIndex >= _questions.Count;

        public QuizService()
        {
            BuildQuestionBank();
        }

        private void BuildQuestionBank()
        {
            _questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    QuestionText = "What should you do if you receive an email asking for your password?",
                    Type = QuestionType.MultipleChoice,
                    Options = new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it" },
                    CorrectAnswer = "C",
                    Explanation = "Reporting phishing emails helps prevent scams and protects others too."
                },
                new QuizQuestion
                {
                    QuestionText = "A strong password should include uppercase, lowercase, numbers, and symbols.",
                    Type = QuestionType.TrueFalse,
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "True",
                    Explanation = "Mixing character types makes passwords significantly harder to crack."
                },
                new QuizQuestion
                {
                    QuestionText = "Which of these is the safest way to connect on public Wi-Fi?",
                    Type = QuestionType.MultipleChoice,
                    Options = new List<string> { "A) Connect directly, no precautions", "B) Use a VPN", "C) Disable your antivirus", "D) Share your password with strangers nearby" },
                    CorrectAnswer = "B",
                    Explanation = "A VPN encrypts your traffic, protecting your data on unsecured public networks."
                },
                new QuizQuestion
                {
                    QuestionText = "It is safe to reuse the same password across multiple accounts.",
                    Type = QuestionType.TrueFalse,
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "False",
                    Explanation = "Reusing passwords means one breach can compromise all your accounts."
                },
                new QuizQuestion
                {
                    QuestionText = "What is 'phishing'?",
                    Type = QuestionType.MultipleChoice,
                    Options = new List<string> { "A) A type of antivirus software", "B) A fraudulent attempt to obtain sensitive information", "C) A firewall setting", "D) A type of strong password" },
                    CorrectAnswer = "B",
                    Explanation = "Phishing tricks victims into revealing sensitive data by impersonating trusted sources."
                },
                new QuizQuestion
                {
                    QuestionText = "Two-Factor Authentication (2FA) adds an extra layer of account security.",
                    Type = QuestionType.TrueFalse,
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "True",
                    Explanation = "2FA requires a second verification step, protecting accounts even if a password is stolen."
                },
                new QuizQuestion
                {
                    QuestionText = "Which of these is a common sign of a phishing email?",
                    Type = QuestionType.MultipleChoice,
                    Options = new List<string> { "A) Urgent language demanding immediate action", "B) A personalised greeting from a known colleague", "C) No links or attachments", "D) Perfect grammar and spelling" },
                    CorrectAnswer = "A",
                    Explanation = "Phishing emails often create false urgency to pressure victims into acting without thinking."
                },
                new QuizQuestion
                {
                    QuestionText = "Social engineering relies on manipulating people rather than exploiting software.",
                    Type = QuestionType.TrueFalse,
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "True",
                    Explanation = "Social engineering targets human psychology — trust, fear, or urgency — instead of technical flaws."
                },
                new QuizQuestion
                {
                    QuestionText = "What should you check before clicking a link in an email?",
                    Type = QuestionType.MultipleChoice,
                    Options = new List<string> { "A) The font colour", "B) The actual URL by hovering over it", "C) The time it was sent", "D) Nothing, just click it" },
                    CorrectAnswer = "B",
                    Explanation = "Hovering over a link reveals its real destination, which often differs from the displayed text."
                },
                new QuizQuestion
                {
                    QuestionText = "Antivirus software guarantees 100% protection against all malware.",
                    Type = QuestionType.TrueFalse,
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "False",
                    Explanation = "Antivirus software reduces risk significantly but cannot guarantee complete protection."
                },
                new QuizQuestion
                {
                    QuestionText = "What does 'HTTPS' in a website address indicate?",
                    Type = QuestionType.MultipleChoice,
                    Options = new List<string> { "A) The site is encrypted and more secure", "B) The site is hosted in South Africa", "C) The site has no ads", "D) The site loads faster" },
                    CorrectAnswer = "A",
                    Explanation = "HTTPS encrypts data transmitted between your browser and the website."
                },
                new QuizQuestion
                {
                    QuestionText = "Using a password manager is considered a poor security practice.",
                    Type = QuestionType.TrueFalse,
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "False",
                    Explanation = "Password managers generate and store strong, unique passwords securely — they're a recommended practice."
                }
            };

            Shuffle(_questions);
        }

        private void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public void Restart()
        {
            _currentIndex = 0;
            _score = 0;
            Shuffle(_questions);
        }

        public QuizQuestion GetCurrentQuestion()
        {
            if (IsFinished) return null;
            return _questions[_currentIndex];
        }

        public (bool isCorrect, string explanation) SubmitAnswer(string answer)
        {
            var question = GetCurrentQuestion();
            if (question == null) return (false, "Quiz already finished.");

            bool isCorrect = string.Equals(
                answer.Trim(),
                question.CorrectAnswer.Trim(),
                StringComparison.OrdinalIgnoreCase);

            if (isCorrect) _score++;

            _currentIndex++;
            return (isCorrect, question.Explanation);
        }

        public string GetFinalFeedback()
        {
            double percentage = (double)_score / _questions.Count * 100;

            if (percentage >= 80)
                return $"🏆 Excellent! You scored {_score}/{_questions.Count}. You're a cybersecurity pro!";
            if (percentage >= 50)
                return $"👍 Good effort! You scored {_score}/{_questions.Count}. Keep learning to stay even safer online.";

            return $"📘 You scored {_score}/{_questions.Count}. Don't worry — review the tips and try again to sharpen your skills!";
        }
    }
}
