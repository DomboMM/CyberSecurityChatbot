# Cybersecurity Awareness Chatbot

## Project Overview
This project is a Cybersecurity Awareness Chatbot built using C# and WPF (.NET).  
It is designed to educate users about cybersecurity topics such as password safety, phishing, privacy, and online scams.
The chatbot provides an interactive GUI experience with keyword recognition, memory, sentiment detection, dynamic responses, a Task Assistant with reminders, a cybersecurity quiz mini-game, and an Activity Log — combining Parts 1, 2, and 3 into one integrated application.

---

## Features

### GUI Interface
- Built using WPF (Windows Presentation Foundation)
- Modern pink and purple themed interface
- User-friendly chat layout with input box and send button
- Additional tabs for Task Assistant, Quiz, and Activity Log (Part 3)

### Chatbot Intelligence
- Keyword recognition (password, phishing, privacy, scam)
- Randomized responses for variety
- Conversation flow support ("tell me more", "another")

### Memory Feature
- Remembers user interests
- Can recall what the user likes during conversation

### Sentiment Detection
- Detects emotions like:
  - Worried
  - Frustrated
  - Curious
- Adjusts responses to be more supportive

### Input Validation
- Handles empty inputs
- Prevents crashes from invalid entries

---

## Voice Greeting (Part 1 Feature)
- Plays a WAV audio file when the application starts
- Welcomes the user to the chatbot

---

## Task Assistant with Reminders (Part 3)
- Add, view, and manage cybersecurity-related tasks and reminders from the GUI
- Input validation (e.g. empty task title handling)
- Reminders are surfaced to the user within the app

## Task Assistant — Database Integration (Part 3)
- Tasks are saved to a **MySQL** database, not just kept in memory
- Supports adding, viewing, updating, and deleting tasks
- Data persists across app restarts

## Cybersecurity Mini-Game (Quiz) with GUI (Part 3)
- Multiple-choice quiz built into its own GUI tab
- Tracks score and gives feedback on correct/incorrect answers
- Reinforces cybersecurity awareness concepts from the chatbot

## Activity Log Feature with GUI (Part 3)
- Logs key actions (tasks added, quiz attempts, chatbot use) with timestamps
- Viewable directly within the GUI as a running history

## Combining Parts 1, 2, and 3
- All features run inside a single WPF application
- The chatbot logic from Parts 1 & 2 powers the Chat tab inside the Part 3 GUI
- Task Assistant, Quiz, and Activity Log are additional tabs in the same app, not separate programs

---

## Project Structure
- `MainWindow.xaml` → GUI design
- `MainWindow.xaml.cs` → Chatbot, Task Assistant, Quiz, and Activity Log logic
- `greeting.wav` → Voice greeting file
- `.gitignore` → Ignores build files (bin/obj)

---

## How to Run
1. Open the project in Visual Studio
2. Ensure MySQL Server is installed and running locally
3. Update the database connection string with your local MySQL credentials
4. Restore dependencies if needed
5. Build the project
6. Run using F5

Part 2 video: https://youtu.be/dg1OX7JvVX8
Part 3 video: **[ADD LINK HERE]**

---

## GitHub Repository Features
- Multiple meaningful commits across Parts 1, 2, and 3
- Tagged releases for each part (see Releases page)
- Clean project structure
- GitHub version control used correctly

---

## Example Usage
**User:** password  
**Bot:** Use strong passwords with symbols and numbers.

**User:** I am worried about scams  
**Bot:** It's understandable to feel worried. Let me help you stay safe online.

---

## Developer Notes
This project demonstrates:
- Object-Oriented Programming (OOP)
- Event-driven programming (WPF)
- Data structures (Dictionaries, Lists)
- Basic AI-style chatbot logic
- Database integration (MySQL)
- UI/UX design principles

---

