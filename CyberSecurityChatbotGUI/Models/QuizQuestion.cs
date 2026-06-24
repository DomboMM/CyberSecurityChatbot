using System.Collections.Generic;

namespace CyberSecurityChatbotGUI.Models
{
    public enum QuestionType
    {
        MultipleChoice,
        TrueFalse
    }

    public class QuizQuestion
    {
        public string QuestionText { get; set; }
        public QuestionType Type { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }
    }
}
