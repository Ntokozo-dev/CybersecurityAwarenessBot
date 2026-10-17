# PROG6221 Programming 2A - POE Part 2

## Student Details

- **Name:** Ntokozo Mtsali
- **Student number:** ST10498443
- **Module:** PROG6221 Programming 2A
- **Repository:** https://github.com/Ntokozo-dev/CybersecurityAwarenessBot

## Project Overview

The Cybersecurity Awareness Bot teaches users safe digital habits. **Part 1** was a command-line chatbot with a voice greeting, ASCII art, name capture and basic answers. **Part 2** (this version) turns it into a **WPF graphical application** and adds keyword recognition, random responses, conversation flow, memory, sentiment detection and error handling.

All Part 1 features were carried into the GUI:

| Part 1 feature | How it works in Part 2 |

| Voice greeting | `greeting.wav` plays when the window first loads (non-blocking, so the window stays responsive) |
| ASCII art | Displayed in a bordered banner at the top of the window |
| User name | Asked for on startup, validated by `UserProfile`, used in later replies |
| Basic responses | Expanded into dictionary-based keyword responses plus small talk ("how are you", "what is your purpose") |
| Input validation | Blank or unrecognised input gets a helpful message, never a crash |

## Part 2 Features

- **Keyword recognition:** finds cybersecurity topics inside normal sentences, in any letter case.
- **Random responses:** each topic has several tips, and one is chosen at random (never the same tip twice in a row).
- **Conversation flow:** follow-ups such as "tell me more" or "give me another tip" continue the current topic.
- **Memory and recall:** remembers the user's name, favourite topic and the topics discussed.
- **Sentiment detection:** recognises worried, frustrated, unsure and curious wording, then responds with empathy **and** a useful tip in the same reply.
- **Error handling:** blank input and unknown input produce friendly messages.
- **Enter key support:** pressing Enter sends the message.

## GUI Overview

| Control | Name | Purpose |

| Label | `lblTitle` | Application title |
| Banner | `tbBanner` | ASCII art identity (carried over from Part 1) |
| RichTextBox | `rtbChatHistory` | Read-only conversation: user messages in white, bot messages in green |
| TextBox | `txtUserInput` | Where the user types |
| Button | `btnSend` | Sends the message (also triggered by Enter) |

The interface uses a dark terminal theme (dark background, cyan and green text, monospaced font).

## Cybersecurity Topics Supported

The bot recognises these topics, each with four different tips:

1. Passwords
2. Phishing
3. Scams
4. Privacy
5. Safe browsing
6. Two-factor authentication
7. Malware

Each topic is detected by several keywords, for example the passwords topic reacts to "password", "passphrase" and "passcode".

## Memory and Sentiment Features

**Memory**
- Say *"My name is Sam"* (or just *"Sam"*) and the bot uses your name in later replies.
- Say *"I am interested in privacy"* and the bot stores privacy as your favourite topic. Later, when you ask about other topics, it adds a personalised reminder such as *"As someone interested in privacy, you might want to review the security settings on your accounts."*
- Ask *"What do you remember?"* to see your name, favourite topic and the topics discussed so far.

**Sentiment**
- Detected moods: worried, frustrated, unsure, curious.
- The bot opens with a supportive sentence, then gives a tip straight away. The user never has to ask twice.

## How to Run the Project

**Requirements**
- Windows 10 or 11 (WPF is Windows-only)
- Visual Studio 2022 with the **.NET desktop development** workload
- .NET 8 SDK
- Speakers or headphones for the voice greeting

**Steps**
1. Clone the repository: `git clone https://github.com/Ntokozo-dev/CybersecurityAwarenessBot.git`
2. Open **`CybersecurityAwarenessChatbotGUI.sln`** in Visual Studio.
3. In Solution Explorer, make sure **CybersecurityAwarenessChatbotGUI** is the startup project (right-click it, then *Set as Startup Project*).
4. Press **F5** (or Ctrl + F5) to build and run.
5. The window opens and the voice greeting plays. Type your name and click **Send** (or press Enter).

No file paths need changing. `greeting.wav` is part of the project and is copied next to the program when it builds.

## Usage Examples

Replies are random, so the exact wording of tips will vary.

```
Bot: What is your name?
You: My name is Sam
Bot: Nice to meet you, Sam! Ask me about passwords, phishing, scams, privacy, safe browsing, two-factor authentication or malware.

You: Tell me about password safety
Bot: Aim for long passphrases (four or more random words) rather than short, complicated passwords.

You: Tell me more
Bot: Sure! More on passwords: Use a strong, unique password for every account so one leak can't unlock everything.

You: I am interested in privacy
Bot: Great! I'll remember that you're interested in privacy. Here's a first tip: ...

You: I'm worried about scams
Bot: It's completely understandable to feel worried about scams, Sam. Let me share a tip to help you stay safe. ...

You: What do you remember?
Bot: Your name is Sam. You're interested in privacy. So far we've talked about passwords, privacy, scams.
```

**Suggested test inputs**

| Test input | Expected result |
|---|---|
| `Tell me about password safety` | Password tip |
| `I received a scam message` | Scam tip |
| `HOW DO I CREATE A PASSWORD?` | Password tip (capital letters ignored) |
| `Give me another tip` / `Tell me more` | Another tip on the same topic |
| `I am worried about scams` | Empathy plus scam tip |
| `I am frustrated with scam messages` | Support plus scam tip |
| Click Send with an empty box | Helpful message, no crash |
| `blah blah` | Friendly "try rephrasing" message |

## Project Structure

```
CybersecurityAwarenessBot/
|-- .github/workflows/          GitHub Actions (.NET CI)
|-- README.md
|-- CybersecurityAwarenessChatbotGUI/   Part 2 WPF project
|   |-- AsciiArt.cs                     ASCII art (from Part 1)
|   `-- greeting.wav                    Voice greeting audio
|   |-- KeywordResponder.cs             Dictionaries: keywords, tips, small talk
|   |-- MainWindow.xaml                 GUI layout
|   |-- MainWindow.xaml.cs              GUI events (reads input, shows messages)
|   |-- MemoryStore.cs                  Favourite topic, current topic, topics discussed
|   |-- ResponseHandler.cs              Chatbot logic, uses a delegate to send replies
|   |-- SentimentDetector.cs            Mood detection and empathetic openings
|   |-- TextHelper.cs                   Shared text-matching helpers
|   |-- UserProfile.cs                  Stores and validates the user's name (from Part 1)
|   |-- VoiceGreeting.cs                Plays the WAV greeting (from Part 1)

### Technical notes

- **Collections:** `Dictionary<string, List<string>>` stores topic keywords and responses, so adding a topic means adding data, not new `if` statements.
- **Random selection:** one `Random` object is created per class and reused, and the last tip is remembered so tips don't repeat back to back.
- **Delegate:** `BotMessageHandler` is declared in `ResponseHandler.cs`. The window plugs its `DisplayBotMessage` method into it, so the chatbot logic can send a reply without knowing how the window displays it.
- **Separation of concerns:** the window only handles the GUI; keyword, sentiment and memory logic live in their own classes.

## GitHub / CI Information

- GitHub Actions workflow **.NET CI** builds the project on every push.
- Commits follow the development order: project setup, GUI layout, Part 1 migration, keyword recognition, random responses, memory, sentiment, error handling, README.
- Check the **Actions** tab for the latest run.

## Tags and Releases

| Tag | Description |
|---|---|
| `v2.0-gui` | Part 2 GUI created and Part 1 features migrated |
| `v2.1-dynamic-responses` | Keyword recognition, random responses and conversation flow |
| `v2.2-memory-sentiment` | Memory, sentiment detection, validation and final polish |
| `v2.3-final` | Ascii banner editing

## Video Presentation Link
