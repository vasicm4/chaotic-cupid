using System.Security.Cryptography;
using ChaoticCupid.Server.Hubs;
using ChaoticCupid.Server.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChaoticCupid.Server.Services;

public class CupidBackgroundWorker : BackgroundService
{
    private readonly IHubContext<CupidHub> _hubContext;

    private readonly string[] _messages =
    {
        "I look forward to our meeting!",
        "I want us to get to know each other.",
        "I am not interested in getting to know you."
    };

    public CupidBackgroundWorker(IHubContext<CupidHub> hubContext)
    {
        _hubContext = hubContext;
    }

protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(60000, stoppingToken);

                var allUsers = CupidHub.People.Values.ToList();

                foreach (var recipient in allUsers)
                {
                    if (recipient.IsWaitingForAck) continue;

                    Person? bestPartner = null;
                    int maxScore = -1;

                    foreach (var potentialPartner in allUsers)
                    {
                        if (potentialPartner.ConnectionId == recipient.ConnectionId) continue;

                        if (recipient.BlockedUsers.Contains(potentialPartner.Username)) continue;

                        int score = 0;

                        if (recipient.City.Equals(potentialPartner.City, StringComparison.OrdinalIgnoreCase))
                            score += 30;

                        if (Math.Abs(recipient.Age - potentialPartner.Age) <= 2)
                            score += 20;

                        int randomFactor = RandomNumberGenerator.GetInt32(0, 101); 
                        score += randomFactor;

                        if (score > maxScore)
                        {
                            maxScore = score;
                            bestPartner = potentialPartner;
                        }
                    }

                    if (bestPartner != null)
                    {
                        int randomIndex = RandomNumberGenerator.GetInt32(0, _messages.Length);
                        string message = _messages[randomIndex];

                        string phoneToSend = (message == "I am not interested in getting to know you.") 
                            ? "[HIDDEN]" 
                            : bestPartner.Phone;

                        recipient.IsWaitingForAck = true;

                        await _hubContext.Clients.Client(recipient.ConnectionId).SendAsync(
                            "LetterReceived", 
                            bestPartner.Username, 
                            bestPartner.City, 
                            bestPartner.Age, 
                            phoneToSend, 
                            message
                        );
                    }
                }
            }
        }
}
