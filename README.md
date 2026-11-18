# PROG6221 Cybersecurity Awareness Chatbot - Final POE

## Student Details

- **Name:** Ntokozo Mtsali
- **Student number:** ST10498443
- **Module:** PROG6221 Programming 2A
- **Repository:** https://github.com/Ntokozo-dev/CybersecurityAwarenessBot
- **Lecturer:** Ms A Phewa

## Project Overview
This is a WPF desktop application: a Cybersecurity Awareness Assistant that combines the work from Parts 1, 2 and 3. Users can chat about cybersecurity topics, manage cybersecurity tasks with reminders (stored in MySQL), take a cybersecurity quiz, give natural-language commands, and review a log of their recent actions.

## Final Features

### Part 1
- Voice greeting on start-up
- ASCII art banner
- Basic cybersecurity interaction
- Name capture with input validation

### Part 2
- WPF GUI with a chat history window
- Keyword recognition across cybersecurity topics (passwords, phishing, privacy, scams)
- Random responses so answers vary
- Conversation flow, memory and recall (`MemoryStore`)
- Sentiment detection (`SentimentDetector`)
- Delegate used to pass chatbot replies to the GUI

### Part 3
- **Task Assistant (MySQL):** add tasks with a title, description and optional reminder date; view, complete and delete tasks. All changes are saved to MySQL and the list always reloads from the database.
- **Reminders:** tasks whose reminder is due or overdue are shown in a pop-up when the application opens.
- **Cybersecurity Quiz:** one question at a time, immediate feedback on every answer, and a final score shown as a percentage, with a retake option.
- **NLP simulation:** simple string and phrase matching (`IntentRecognizer`) so different wordings reach the same feature. Tasks can be created straight from chat, including a reminder date ("tomorrow", "today", "next week", "in 3 days").
- **Activity Log:** records tasks, reminders, quiz activity and NLP commands. The tab shows the 5 most recent actions, with **Show More** for the last 10. It refreshes automatically as actions happen.

## Technologies
- C# / .NET, WPF
- MySQL 8.0
- Visual Studio 2022

## Database Setup
1. Install and start MySQL Server (MySQL Workbench is useful for checking the data).
2. Run the SQL below to create the database and table (also provided as `database_setup.sql` if included in the repository):

   ```sql
   CREATE DATABASE IF NOT EXISTS cybersecurity_db;
   USE cybersecurity_db;

   CREATE TABLE IF NOT EXISTS tasks (
       TaskId      INT NOT NULL AUTO_INCREMENT,
       Title       VARCHAR(100) NOT NULL,
       Description VARCHAR(255) DEFAULT NULL,
       Reminder    DATETIME DEFAULT NULL,
       IsCompleted TINYINT(1) DEFAULT 0,
       PRIMARY KEY (TaskId)
   ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
   ```
3. Open `TaskDatabase.cs` and replace `YOUR_PASSWORD` in the connection string with your local MySQL password.
   > Do not commit a real password to a public repository. The submitted code uses a placeholder.
4. Build and run the application in Visual Studio (**F5**).

## How to Use
- **Chat tab:** enter your name, then ask about cybersecurity topics or type a command.
- **Tasks tab:** type a title, description and optional reminder date, then click **Add Task**. Select a task to **Mark Complete** or **Delete** it.
- **Quiz tab:** click **Start Quiz**, choose an answer, click **Submit Answer**, then **Next Question**. Your score appears at the end.
- **Activity Log tab:** shows recent actions. Use **Refresh** for the last 5 and **Show More** for the last 10.

### Example chat commands
| You type | What happens |
|---|---|
| Add a task to review my privacy settings | Task is saved to the database |
| Remind me to update my password tomorrow | Task is saved with tomorrow's date as the reminder |
| Show my tasks | Switches to the Tasks tab |
| Start the cyber quiz | Switches to the Quiz tab |
| What have you done for me? | Switches to the Activity Log tab |
| Tell me about phishing | Normal chatbot response |

## Project Structure
| File | Purpose |
|---|---|
| `MainWindow.xaml` / `MainWindow.xaml.cs` | GUI layout and event handling for all tabs |
| `UserProfile.cs`, `ResponseHandler.cs` | Part 1 classes reused in the GUI |
| `KeywordResponder.cs`, `MemoryStore.cs`, `SentimentDetector.cs`, `TextHelper.cs` | Part 2 chatbot logic |
| `TaskDatabase.cs`, `CyberTask.cs` | MySQL access and the task model |
| `QuizManager.cs`, `QuizQuestion.cs`, `QuizAnswerResult.cs` | Quiz logic and models |
| `UserIntent.cs` | `UserIntent` enum and `IntentRecognizer` (NLP simulation) |
| `ActivityLogger.cs`, `ActivityEntry.cs` | Activity Log |
| `AsciiArt.cs`, `VoiceGreeting.cs` | Part 1 greeting features |

## Releases
- `v1.0-part1`
- `v2.0-part2`
- `v3.0-final`

## Video Presentation
YOUTUBE LINK HERE (unlisted)

