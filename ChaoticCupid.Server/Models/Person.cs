namespace ChaoticCupid.Server.Models;

public class Person
{
    public string ConnectionId { get; set; }
    public string Username { get; set; }
    public string City { get; set; }
    public int Age { get; set; }
    public string Phone { get; set; }
    public bool IsWaitingForAck { get; set; } = false;
    public List<string> BlockedUsers { get; set; } = new List<string>();

    public Person()
    {
    }
}