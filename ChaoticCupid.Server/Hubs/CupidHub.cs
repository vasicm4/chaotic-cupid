using System.Collections.Concurrent;
using ChaoticCupid.Server.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChaoticCupid.Server.Hubs;

public class CupidHub : Hub
{
    public static ConcurrentDictionary<string, Person> People = new ConcurrentDictionary<string, Person>();
    public void InitSinglePerson(string username, string city, int age, string phone)
    {
        var person = new Person
        {
            ConnectionId = Context.ConnectionId,
            Username = username,
            City = city,
            Age = age,
            Phone = phone
        };
            
        People[Context.ConnectionId] = person;
        Console.WriteLine($"[Server] User registered: {username} ({city}, {age} y/o)");
    }
    public void ConfirmReceipt()
    {
        if (People.TryGetValue(Context.ConnectionId, out var person))
        {
            person.IsWaitingForAck = false;
            Console.WriteLine($"[Server] User {person.Username} confirmed letter receipt.");
        }
    }

    public void BlockUser(string usernameToBlock)
    {
        if (People.TryGetValue(Context.ConnectionId, out var person))
        {
            if (!person.BlockedUsers.Contains(usernameToBlock))
            {
                person.BlockedUsers.Add(usernameToBlock);
                Console.WriteLine($"[Server] {person.Username} blocked user {usernameToBlock}.");
            }
        }
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        People.TryRemove(Context.ConnectionId, out _);
        return base.OnDisconnectedAsync(exception);
    }
}