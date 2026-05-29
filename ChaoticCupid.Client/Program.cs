using Microsoft.Extensions.Configuration;
using System.IO;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();
    
string serverUrl = configuration["CupidServer:BaseUrl"] ?? "http://localhost:5000/cupid";

connection = new HubConnectionBuilder()
    .WithUrl(serverUrl)
    .WithAutomaticReconnect()
    .Build();
    
connection.On<string, string, int, string, string>("LetterReceived", (fromUser, city, age, phone, message) =>
{
    Console.WriteLine("\n========================================");
    Console.WriteLine("YOU RECEIVED A LOVE LETTER!");
    Console.WriteLine($"From: {fromUser}");
    Console.WriteLine($"City: {city}");
    Console.WriteLine($"Age: {age}");
    Console.WriteLine($"Phone Number: {phone}");
    Console.WriteLine($"Message: \"{message}\"");
    Console.WriteLine("========================================");
    Console.WriteLine("Press ENTER to acknowledge receipt and continue receiving letters...");
});

try
{
    await connection.StartAsync();
    Console.WriteLine("Successfully connected to the Cupid service.");
}
catch (Exception ex)
{
    Console.WriteLine($"Connection error: {ex.Message}");
    return;
}

string username = InputText("Enter your username: ");
string city = InputText("Enter your city: ");
int age = InputNumber("Enter your age: ", min: 14, max: 100);
string phone = InputText("Enter your phone number: ");

await connection.InvokeAsync("InitSinglePerson", username, city, age, phone);
Console.WriteLine("\nSuccessfully registered with Cupid! Waiting for your soulmate...\n");

while (true)
{
    string input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        await connection.InvokeAsync("ConfirmReceipt");
        Console.WriteLine("[System] Letter receipt acknowledged. You are back in the game.");
    }
    else if (input.StartsWith("/block "))
    {
        string userToBlock = input.Substring(7).Trim();
        if (!string.IsNullOrEmpty(userToBlock))
        {
            await connection.InvokeAsync("BlockUser", userToBlock);
            Console.WriteLine($"[System] You have blocked user: {userToBlock}");
        }
    }
    else
    {
        Console.WriteLine("[System] Unknown command. To block someone use: /block username");
    }
}

private static string InputText(string message)
{
    while (true)
    {
        Console.Write(message);
        string input = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(input)) return input;
        Console.WriteLine("Error: Input cannot be empty!");
    }
}

private static int InputNumber(string message, int min, int max)
{
    while (true)
    {
        Console.Write(message);
        string input = Console.ReadLine();
        if (int.TryParse(input, out int number))
        {
            if (number >= min && number <= max) return number;
            Console.WriteLine($"Error: Number must be between {min} and {max} (negative numbers are not allowed)!");
        }
        else
        {
            Console.WriteLine("Error: Please enter a valid numeric character!");
        }
    }
}