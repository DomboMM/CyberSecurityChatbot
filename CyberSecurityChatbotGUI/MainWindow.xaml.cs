using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;

namespace CyberSecurityChatbotGUI
{
    public partial class MainWindow : Window
    {
        // Store chatbot responses
        private Dictionary<string, List<string>> responses;

        // Random responses
        private Random random = new Random();

        // Memory variables
        private string lastTopic = "";
        private string userInterest = "";

        public MainWindow()
        {
            InitializeComponent();

            SetupResponses();

            // Play greeting sound
            PlayGreeting();
        }

        // Greeting sound method
        private void PlayGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer(
                 System.IO.Path.Combine(
                  AppDomain.CurrentDomain.BaseDirectory, "greeting1.wav")
 );
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Audio error: " + ex.Message);
            }
        }

        // Cybersecurity responses
        private void SetupResponses()
        {
            responses = new Dictionary<string, List<string>>()
            {
                {
                    "password",
                    new List<string>()
                    {
                        "Use strong passwords with symbols and numbers.",
                        "Avoid using personal information in passwords.",
                        "Use different passwords for every account."
                    }
                },

                {
                    "phishing",
                    new List<string>()
                    {
                        "Do not click suspicious email links.",
                        "Scammers pretend to be trusted companies.",
                        "Always verify emails before responding."
                    }
                },

                {
                    "privacy",
                    new List<string>()
                    {
                        "Review your privacy settings often.",
                        "Avoid sharing personal information online.",
                        "Use secure websites with HTTPS."
                    }
                },

                {
                    "scam",
                    new List<string>()
                    {
                        "Be careful of online scams asking for money.",
                        "Never share banking details with strangers.",
                        "Scammers often create fake urgency."
                    }
                }
            };
        }

        // Send button
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInput.Text.ToLower();

            // Input validation
            if (string.IsNullOrWhiteSpace(input))
            {
                ChatList.Items.Add("Bot: Please enter a message.");
                return;
            }

            // Show user message
            ChatList.Items.Add("You: " + input);

            // Get chatbot response
            string response = GetResponse(input);

            // Show chatbot response
            ChatList.Items.Add("Bot: " + response);

            // Clear textbox
            UserInput.Clear();
        }

        // Chatbot logic
        private string GetResponse(string input)
        {
            // Greetings
            if (input.Contains("hello") || input.Contains("hi"))
            {
                return "Hello! How can I help you with cybersecurity today?";
            }

            // Purpose
            if (input.Contains("purpose"))
            {
                return "My purpose is to help users stay safe online and learn Sabout cybersecurity.";
            }

            // Sentiment detection
            if (input.Contains("worried"))
            {
                return "It's understandable to feel worried. Let me help you stay safe online.";
            }

            if (input.Contains("frustrated"))
            {
                return "I understand this can feel frustrating. Cybersecurity can be complicated sometimes.";
            }

            if (input.Contains("curious"))
            {
                return "Curiosity is great! Learning cybersecurity helps protect your information.";
            }

            // Keyword recognition
            foreach (var key in responses.Keys)
            {
                if (input.Contains(key))
                {
                    lastTopic = key;

                    // Memory feature
                    if (input.Contains("like") || input.Contains("interested"))
                    {
                        userInterest = key;

                        return $"Great! I’ll remember that you are interested in {key}.";
                    }

                    // Random responses
                    List<string> list = responses[key];

                    return list[random.Next(list.Count)];
                }
            }

            // Conversation flow
            if (input.Contains("more") || input.Contains("another"))
            {
                if (!string.IsNullOrEmpty(lastTopic))
                {
                    List<string> list = responses[lastTopic];

                    return list[random.Next(list.Count)];
                }

                return "Please ask about a cybersecurity topic first.";
            }

            // Memory recall
            if (input.Contains("remember") || input.Contains("what do i like"))
            {
                if (!string.IsNullOrEmpty(userInterest))
                {
                    return $"You told me you are interested in {userInterest}.";
                }

                return "I don’t remember your interests yet.";
            }

            // Default response
            return "I’m not sure I understand. Can you rephrase?";
        }
    }
}