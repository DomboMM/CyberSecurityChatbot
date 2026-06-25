using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CyberSecurityChatbotGUI.Models;
using CyberSecurityChatbotGUI.Services;

namespace CyberSecurityChatbotGUI
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, List<string>> responses;
        private Random random = new Random();
        private string lastTopic = "";
        private string userInterest = "";

        private DatabaseHelper _db;
        private ActivityLogService _activityLog;
        private QuizService _quiz;
        private NlpHelper _nlp;

        private List<RadioButton> _currentQuizOptions = new List<RadioButton>();

        public MainWindow()
        {
            InitializeComponent();

            SetupResponses();
            PlayGreeting();

            _db = new DatabaseHelper();
            _activityLog = new ActivityLogService(_db);
            _quiz = new QuizService();
            _nlp = new NlpHelper();

            CheckDatabaseConnection();
            RefreshTasks_Click(null, null);
            RefreshLog_Click(null, null);
        }

        private void CheckDatabaseConnection()
        {
            if (!_db.TestConnection(out string error))
            {
                MessageBox.Show(
                    "Could not connect to the MySQL database.\n\n" +
                    "Please make sure:\n" +
                    "1. MySQL Server is running\n" +
                    "2. The 'cybersecurity_chatbot' database exists\n" +
                    "3. Your password in DatabaseHelper.cs is correct\n\n" +
                    "Error: " + error,
                    "Database Connection Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void PlayGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting1.wav"));
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Audio error: " + ex.Message);
            }
        }

        private void SetupResponses()
        {
            responses = new Dictionary<string, List<string>>()
            {
                { "password", new List<string>() {
                    "Use strong passwords with symbols and numbers.",
                    "Avoid using personal information in passwords.",
                    "Use different passwords for every account.",
                    "Consider using a password manager for better security.",
                    "Never share your passwords with anyone." } },

                { "phishing", new List<string>() {
                    "Do not click suspicious email links.",
                    "Scammers pretend to be trusted companies.",
                    "Always verify emails before responding.",
                    "Phishing emails often create panic to trick users.",
                    "Check email addresses carefully before clicking links." } },

                { "privacy", new List<string>() {
                    "Review your privacy settings often.",
                    "Avoid sharing personal information online.",
                    "Use secure websites with HTTPS.",
                    "Enable two-factor authentication for better privacy.",
                    "Be careful when using public Wi-Fi networks." } },

                { "scam", new List<string>() {
                    "Be careful of online scams asking for money.",
                    "Never share banking details with strangers.",
                    "Scammers often create fake urgency.",
                    "Do not trust messages promising free prizes.",
                    "Online scams often pretend to be official organisations." } },

                { "vpn", new List<string>() {
                    "Using a VPN helps protect your online privacy.",
                    "VPNs encrypt your internet connection.",
                    "A VPN can help protect your information on public Wi-Fi." } },

                { "malware", new List<string>() {
                    "Install antivirus software to protect your device.",
                    "Avoid downloading files from unknown websites.",
                    "Keep your software updated to prevent malware attacks." } },

                { "2fa", new List<string>() {
                    "Two-factor authentication adds extra account security.",
                    "Enable 2FA on email and banking accounts.",
                    "2FA helps protect accounts even if passwords are stolen." } },

                { "wifi", new List<string>() {
                    "Avoid entering passwords on public Wi-Fi.",
                    "Use a VPN when using public internet connections.",
                    "Public Wi-Fi can be risky if not secured." } }
            };
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Send_Click(sender, e);
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string rawInput = UserInput.Text;
            string input = rawInput.ToLower();

            if (string.IsNullOrWhiteSpace(input))
            {
                ChatList.Items.Add("Bot: Please enter a message.");
                return;
            }

            ChatList.Items.Add("You: " + rawInput);
            UserInput.Clear();

            Intent intent = _nlp.DetectIntent(rawInput);

            string response;
            switch (intent)
            {
                case Intent.AddTask:
                    response = HandleNlpAddTask(rawInput);
                    break;

                case Intent.SetReminder:
                    response = HandleNlpSetReminder(rawInput);
                    break;

                case Intent.ViewTasks:
                    response = HandleNlpViewTasks();
                    break;

                case Intent.StartQuiz:
                    response = "Great! Head over to the 🎮 Quiz tab and click 'Start Quiz' to test your knowledge!";
                    break;

                case Intent.ShowActivityLog:
                    response = _activityLog.GetFormattedSummary();
                    break;

                default:
                    response = GetResponse(input);
                    break;
            }

            ChatList.Items.Add("Bot: " + response);
            ChatList.ScrollIntoView(ChatList.Items[ChatList.Items.Count - 1]);
        }

        private string GetResponse(string input)
        {
            if (input.Trim().Equals("hello") ||
                input.Trim().Equals("hi"))
                return "Hello! Welcome to the Cybersecurity Awareness Chatbot. How can I help you today?";

            if (input.Contains("purpose"))
                return "My purpose is to help users stay safe online and learn about cybersecurity.";

            if (input.Contains("worried"))
            {
                _activityLog.Log("Sentiment detected: Worried", "Sentiment");

                if (input.Contains("phishing"))
                    return "It's understandable to feel worried. Be careful of suspicious emails and never click unknown links.";

                if (input.Contains("password"))
                    return "It's understandable to feel worried. Use strong passwords and enable two-factor authentication.";

                if (input.Contains("privacy"))
                    return "It's understandable to feel worried. Review your privacy settings regularly.";

                return "It's understandable to feel worried. Let me help you stay safe online.";
            }

            if (input.Contains("frustrated"))
            {
                _activityLog.Log("Sentiment detected: Frustrated", "Sentiment");

                return "I understand this can feel frustrating. Cybersecurity can be complicated sometimes.";
            }

            if (input.Contains("curious"))
            {
                _activityLog.Log("Sentiment detected: Curious", "Sentiment");

                return "Curiosity is great! Learning cybersecurity helps protect your information.";
            }

            foreach (var key in responses.Keys)
            {
                if (input.Contains(key))
                {
                    lastTopic = key;

                    if (input.Contains("like") || input.Contains("interested"))
                    {
                        userInterest = key;
                        return $"Great! I'll remember that you are interested in {key}.";
                    }

                    List<string> list = responses[key];
                    return list[random.Next(list.Count)];
                }
            }

            if (input.Contains("more") || input.Contains("another"))
            {
                if (!string.IsNullOrEmpty(lastTopic))
                {
                    List<string> list = responses[lastTopic];
                    return list[random.Next(list.Count)];
                }
                return "Please ask about a cybersecurity topic first.";
            }
            if (input.Contains("remember") ||
               input.Contains("what do i like") ||
               input.Contains("what am i interested in") ||
               input.Contains("my interest"))
            {
                if (!string.IsNullOrEmpty(userInterest))
                    return $"You told me you are interested in {userInterest}.";

                return "I don't remember your interests yet.";
            }
            {
                if (!string.IsNullOrEmpty(userInterest))
                    return $"You told me you are interested in {userInterest}.";
                return "I don't remember your interests yet.";
            }

            return "I'm not sure I understand. Can you rephrase?";
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleBox.Text.Trim();
            string desc = TaskDescBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a task title.");
                return;
            }

            DateTime? reminder = null;
            if (int.TryParse(ReminderDaysBox.Text.Trim(), out int days) && days > 0)
            {
                reminder = DateTime.Now.AddDays(days);
            }

            try
            {
                _db.AddTask(title, desc, reminder);

                string logMsg = reminder.HasValue
                    ? $"Task added: '{title}' (Reminder set for {reminder.Value:dd MMM yyyy})"
                    : $"Task added: '{title}' (no reminder set)";

                _activityLog.Log(logMsg, "Task");

                TaskTitleBox.Clear();
                TaskDescBox.Clear();
                ReminderDaysBox.Clear();

                RefreshTasks_Click(null, null);
                RefreshLog_Click(null, null);

                MessageBox.Show("Task added successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding task: " + ex.Message);
            }
        }

        private void RefreshTasks_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TaskList.Items.Clear();
                var tasks = _db.GetAllTasks();

                foreach (var task in tasks)
                {
                    TaskList.Items.Add(task);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading tasks: " + ex.Message);
            }
        }

        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is TaskItem selected)
            {
                _db.MarkTaskCompleted(selected.TaskId);
                _activityLog.Log($"Task marked as completed: '{selected.Title}'", "Task");
                RefreshTasks_Click(null, null);
                RefreshLog_Click(null, null);
            }
            else
            {
                MessageBox.Show("Please select a task first.");
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is TaskItem selected)
            {
                var result = MessageBox.Show($"Delete task '{selected.Title}'?", "Confirm", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _db.DeleteTask(selected.TaskId);
                    _activityLog.Log($"Task deleted: '{selected.Title}'", "Task");
                    RefreshTasks_Click(null, null);
                    RefreshLog_Click(null, null);
                }
            }
            else
            {
                MessageBox.Show("Please select a task first.");
            }
        }

        private string HandleNlpAddTask(string rawInput)
        {
            string title = _nlp.ExtractTaskTitle(rawInput);
            DateTime? reminder = _nlp.ExtractReminderDate(rawInput);

            try
            {
                _db.AddTask(title, $"Added via chat: \"{rawInput}\"", reminder);

                string logMsg = reminder.HasValue
                    ? $"Task added via NLP: '{title}' (Reminder: {reminder.Value:dd MMM yyyy})"
                    : $"Task added via NLP: '{title}' (no reminder set)";

                _activityLog.Log(logMsg, "NLP");
                RefreshTasks_Click(null, null);
                RefreshLog_Click(null, null);

                if (reminder.HasValue)
                    return $"Task added: '{title}'. I'll remind you on {reminder.Value:dd MMM yyyy}.";

                return $"Task added: '{title}'. Would you like to set a reminder? (e.g. \"remind me in 3 days\")";
            }
            catch (Exception ex)
            {
                return "I couldn't add that task — database error: " + ex.Message;
            }
        }

        private string HandleNlpSetReminder(string rawInput)
        {
            string title = _nlp.ExtractTaskTitle(rawInput);
            DateTime? reminder = _nlp.ExtractReminderDate(rawInput) ?? DateTime.Now.AddDays(3);

            try
            {
                _db.AddTask(title, $"Reminder added via chat: \"{rawInput}\"", reminder);
                _activityLog.Log($"Reminder set for '{title}' on {reminder.Value:dd MMM yyyy}", "Reminder");

                RefreshTasks_Click(null, null);
                RefreshLog_Click(null, null);

                return $"Got it! I'll remind you about '{title}' on {reminder.Value:dd MMM yyyy}.";
            }
            catch (Exception ex)
            {
                return "I couldn't set that reminder — database error: " + ex.Message;
            }
        }

        private string HandleNlpViewTasks()
        {
            var tasks = _db.GetAllTasks();
            if (tasks.Count == 0)
                return "You don't have any tasks yet. Try saying 'add a task to enable 2FA'.";

            var pending = tasks.Where(t => !t.IsCompleted).Take(5);
            string list = string.Join("\n", pending.Select((t, i) => $"{i + 1}. {t.Title}"));

            return $"Here are your pending tasks:\n{list}";
        }

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            _quiz.Restart();
            _activityLog.Log("Quiz started", "Quiz");
            RefreshLog_Click(null, null);

            StartQuizButton.IsEnabled = false;
            ShowNextQuizQuestion();
        }

        private void ShowNextQuizQuestion()
        {
            QuizFeedbackText.Text = "";
            QuizOptionsPanel.Children.Clear();
            _currentQuizOptions.Clear();

            if (_quiz.IsFinished)
            {
                QuizQuestionText.Text = _quiz.GetFinalFeedback();
                QuizProgressText.Text = "Quiz Complete!";
                NextQuestionButton.IsEnabled = false;
                StartQuizButton.IsEnabled = true;
                StartQuizButton.Content = "🔁 Play Again";

                _activityLog.Log($"Quiz completed — score {_quiz.Score}/{_quiz.TotalQuestions}", "Quiz");
                RefreshLog_Click(null, null);
                return;
            }

            var question = _quiz.GetCurrentQuestion();
            QuizProgressText.Text = $"Question {_quiz.CurrentQuestionNumber} of {_quiz.TotalQuestions}  |  Score: {_quiz.Score}";
            QuizQuestionText.Text = question.QuestionText;

            foreach (var option in question.Options)
            {
                var radio = new RadioButton
                {
                    Content = option,
                    GroupName = "QuizOptions",
                    Foreground = Brushes.White,
                    FontSize = 14,
                    Margin = new Thickness(0, 6, 0, 0),
                    Tag = option.StartsWith("A)") || option.StartsWith("B)") || option.StartsWith("C)") || option.StartsWith("D)")
                        ? option.Substring(0, 1)
                        : option
                };
                QuizOptionsPanel.Children.Add(radio);
                _currentQuizOptions.Add(radio);
            }

            NextQuestionButton.IsEnabled = true;
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            var selected = _currentQuizOptions.FirstOrDefault(r => r.IsChecked == true);

            if (selected == null)
            {
                MessageBox.Show("Please select an answer first.");
                return;
            }

            string answer = selected.Tag.ToString();
            var (isCorrect, explanation) = _quiz.SubmitAnswer(answer);

            QuizFeedbackText.Foreground = isCorrect ? Brushes.LightGreen : Brushes.Salmon;
            QuizFeedbackText.Text = (isCorrect ? "✅ Correct! " : "❌ Incorrect. ") + explanation;

            foreach (var r in _currentQuizOptions) r.IsEnabled = false;

            NextQuestionButton.Content = "Next Question ▶";
            NextQuestionButton.Click -= NextQuestion_Click;
            NextQuestionButton.Click += AdvanceQuiz_Click;
        }

        private void AdvanceQuiz_Click(object sender, RoutedEventArgs e)
        {
            NextQuestionButton.Click -= AdvanceQuiz_Click;
            NextQuestionButton.Click += NextQuestion_Click;
            ShowNextQuizQuestion();
        }

        private void RefreshLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ActivityLogList.Items.Clear();
                var logs = _activityLog.GetRecent(10);

                if (logs.Count == 0)
                {
                    ActivityLogList.Items.Add("No activity recorded yet.");
                    return;
                }

                foreach (var log in logs)
                {
                    ActivityLogList.Items.Add(log);
                }
            }
            catch (Exception ex)
            {
                ActivityLogList.Items.Clear();
                ActivityLogList.Items.Add("Error loading activity log: " + ex.Message);
            }
        }
    }
}