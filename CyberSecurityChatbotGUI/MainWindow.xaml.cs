using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;

namespace CyberSecurityChatbotGUI
{
    public partial class MainWindow : Window
    {
        // Dictionary used to store cybersecurity topics and responses
        private Dictionary<string, List<string>> responses;

        // Random object used to select random responses
        private Random random = new Random();

        // Stores the last discussed topic for conversation flow
        private string lastTopic = "";

        // Stores the user's favourite cybersecurity topic
        private string userInterest = "";

        public MainWindow()
        {
            InitializeComponent();

            // Load chatbot responses
            SetupResponses();

            // Play voice greeting when app starts
            PlayGreeting();
        }

        // Method used to play greeting audio
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

        // Method used to set up chatbot responses
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
                        "Use different passwords for every account.",
                        "Consider using a password manager for better security.",
                        "Never share your passwords with anyone."
                    }
                },

                {
                    "phishing",
                    new List<string>()
                    {
                        "Do not click suspicious email links.",
                        "Scammers pretend to be trusted companies.",
                        "Always verify emails before responding.",
                        "Phishing emails often create panic to trick users.",
                        "Check email addresses carefully before clicking links."
                    }
                },

                {
                    "privacy",
                    new List<string>()
                    {
                        "Review your privacy settings often.",
                        "Avoid sharing personal information online.",
                        "Use secure websites with HTTPS.",
                        "Enable two-factor authentication for better privacy.",
                        "Be careful when using public Wi-Fi networks."
                    }
                },

                {
                    "scam",
                    new List<string>()
                    {
                        "Be careful of online scams asking for money.",
                        "Never share banking details with strangers.",
                        "Scammers often create fake urgency.",
                        "Do not trust messages promising free prizes.",
                        "Online scams often pretend to be official organisations."
                    }
                },

                {
                    "vpn",
                    new List<string>()
                    {
                        "Using a VPN helps protect your online privacy.",
                        "VPNs encrypt your internet connection.",
                        "A VPN can help protect your information on public Wi-Fi."
                    }
                },

                {
                    "malware",
                    new List<string>()
                    {
                        "Install antivirus software to protect your device.",
                        "Avoid downloading files from unknown websites.",
                        "Keep your software updated to prevent malware attacks."
                    }
                },

                {
                    "2fa",
                    new List<string>()
                    {
                        "Two-factor authentication adds extra account security.",
                        "Enable 2FA on email and banking accounts.",
                        "2FA helps protect accounts even if passwords are stolen."
                    }
                },

                {
                    "wifi",
                    new List<string>()
                    {
                        "Avoid entering passwords on public Wi-Fi.",
                        "Use a VPN when using public internet connections.",
                        "Public Wi-Fi can be risky if not secured."
                    }
                }
            };
        }

        // Send button click event
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            // Convert user input to lowercase
            string input = UserInput.Text.ToLower();

            // Input validation for empty messages
            if (string.IsNullOrWhiteSpace(input))
            {
                ChatList.Items.Add("Bot: Please enter a message.");
                return;
            }

            // Display user message in chat
            ChatList.Items.Add("You: " + input);

            // Generate chatbot response
            string response = GetResponse(input);

            // Display chatbot response
            ChatList.Items.Add("Bot: " + response);

            // Clear textbox after sending
            UserInput.Clear();
        }

        // Main chatbot response logic
        private string GetResponse(string input)
        {
            // Greeting responses
            if (input.Contains("hello") || input.Contains("hi"))
            {
                return "Hello! Welcome to the Cybersecurity Awareness Chatbot. How can I help you today?";
            }

            // Chatbot purpose response
            if (input.Contains("purpose"))
            {
                return "My purpose is to help users stay safe online and learn about cybersecurity.";
            }

            // Sentiment detection for worried users
            if (input.Contains("worried"))
            {
                return "It's understandable to feel worried. Let me help you stay safe online.";
            }

            // Sentiment detection for frustrated users
            if (input.Contains("frustrated"))
            {
                return "I understand this can feel frustrating. Cybersecurity can be complicated sometimes.";
            }

            // Sentiment detection for curious users
            if (input.Contains("curious"))
            {
                return "Curiosity is great! Learning cybersecurity helps protect your information.";
            }

            // Search for cybersecurity keywords
            foreach (var key in responses.Keys)
            {
                if (input.Contains(key))
                {
                    // Save last topic for follow-up responses
                    lastTopic = key;

                    // Memory feature
                    if (input.Contains("like") || input.Contains("interested"))
                    {
                        userInterest = key;

                        return $"Great! I’ll remember that you are interested in {key}.";
                    }

                    // Get random response from topic list
                    List<string> list = responses[key];

                    return list[random.Next(list.Count)];
                }
            }

            // Conversation flow for follow-up questions
            if (input.Contains("more") || input.Contains("another"))
            {
                if (!string.IsNullOrEmpty(lastTopic))
                {
                    List<string> list = responses[lastTopic];

                    return list[random.Next(list.Count)];
                }

                return "Please ask about a cybersecurity topic first.";
            }

            // Memory recall feature
            if (input.Contains("remember") || input.Contains("what do i like"))
            {
                if (!string.IsNullOrEmpty(userInterest))
                {
                    return $"You told me you are interested in {userInterest}.";
                }

                return "I don’t remember your interests yet.";
            }

            // Default response for unknown input
            return "I’m not sure I understand. Can you rephrase?";
        }
    }
}