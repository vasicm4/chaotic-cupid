# chaotic-cupid

A real-time, peer-to-peer matchmaking simulation where "Chaotic Cupid" acts as a background service routing love letters to users.

## Overview

Chaotic Cupid is a .NET 9 application that simulates a matchmaking service. Users register their profile (username, city, age, phone number) and are periodically matched with other users based on compatibility factors. When a match is found, the system sends a "love letter" from the matched user to the recipient. Recipients must acknowledge receipt of the letter before they can receive another. Users also have the ability to block other users to prevent being matched with them.

The application consists of two parts:
- **Server**: ASP.NET Core SignalR hub with a background worker that performs matchmaking logic.
- **Client**: A console application that connects to the server via SignalR, handles user input, and displays received letters.

## Features

- Real-time communication using ASP.NET Core SignalR
- Background matchmaking service that runs every 60 seconds
- Matching algorithm based on:
  - Same city (+30 points)
  - Age difference ≤ 2 years (+20 points)
  - Random factor (0-100 points)
- Three possible love messages (two positive, one negative)
- Phone number privacy: hidden when the message is negative
- Receipt acknowledgment: users must press Enter to confirm receiving a letter
- User blocking: block specific usernames to avoid being matched with them
- Automatic reconnection handling in the client
- Console-based user interface for registration and interaction

## Technologies

- .NET 9
- ASP.NET Core SignalR
- C#
- BackgroundService (IHostedService)
- ConcurrentDictionary for in-memory user storage

## Architecture

### Server (`ChaoticCupid.Server`)
- **CupidHub** (`Hubs/CupidHub.cs`): SignalR hub that manages user connections, registration, blocking, and receipt confirmation.
- **CupidBackgroundWorker** (`Services/CupidBackgroundWorker.cs`): A background service that runs every minute to:
  - Iterate over all connected users who are not waiting for acknowledgment.
  - For each user, find the best match based on the scoring algorithm.
  - Send a love letter to the matched user via the hub.
- **Person** (`Models/Person.cs`): Data model representing a user, including connection ID, username, city, age, phone, acknowledgment flag, and blocked users list.

### Client (`ChaoticCupid.Client`)
- **Program.cs**: Console application that:
  - Reads server URL from `appsettings.json`.
  - Establishes a SignalR connection to the hub.
  - Registers the user by collecting username, city, age, and phone number.
  - Listens for incoming letters and displays them.
  - Waits for user input: empty line to acknowledge receipt, or `/block username` to block a user.
  - Handles automatic reconnection.

## How to Run

1. **Build the solution**:
   ```bash
   dotnet build
   ```

2. **Run the server**:
   ```bash
   dotnet run --project ChaoticCupid.Server
   ```
   The server will listen on `http://localhost:5000/cupid` (default) or as configured in `appsettings.json` if present.

3. **Run the client** (in a separate terminal):
   ```bash
   dotnet run --project ChaoticCupid.Client
   ```
   The client will prompt for your connection details (username, city, age, phone number) and then connect to the server.

4. **Interaction**:
   - After registering, wait for the background worker to find a match (up to 60 seconds).
   - When a letter arrives, it will be displayed in the console.
   - Press **Enter** to acknowledge receipt and become eligible for the next match.
   - To block a user, type `/block <username>` and press Enter.

## Matching Algorithm Details

The background worker scores each potential partner for a recipient as follows:

- **+30 points** if the recipient and partner are in the same city (case-insensitive).
- **+20 points** if the absolute age difference is ≤ 2 years.
- **+0 to 100 points** random factor (generated using `RandomNumberGenerator`).

The partner with the highest score is selected as the match. If no eligible partners exist (all are blocked or the recipient is waiting for acknowledgment), no letter is sent.

## Love Messages

The system selects one of three messages at random for each letter:

1. "I look forward to our meeting!"
2. "I want us to get to know each other."
3. "I am not interested in getting to know you."

If the third message is selected, the partner's phone number is replaced with `[HIDDEN]` in the letter.

## Notes

- This is a demonstration application and uses in-memory storage only; all data is lost when the server restarts.
- The project showcases real-time web technologies with SignalR and background processing in .NET.

## Future Improvements

- Persistent storage (e.g., database) for user profiles and matching history.
- More sophisticated matching algorithms (machine learning, preferences).
- Web-based client interface (Blazor or JavaScript/HTML).
- Unit tests for matching logic.
- Dockerization for easy deployment.

Enjoy the chaotic romance!