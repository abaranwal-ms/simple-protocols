using System.Net;
using System.Net.Sockets;
using System.Text;

const int Port = 5000;

Console.WriteLine("=== UDP Chat App ===");
Console.WriteLine("1. Start as Receiver");
Console.WriteLine("2. Start as Sender");
Console.Write("Choose mode: ");

var choice = Console.ReadLine();

if (choice == "1")
    await RunReceiver();
else if (choice == "2")
    await RunSender();
else
    Console.WriteLine("Invalid choice.");

async Task RunReceiver()
{
    using var udpClient = new UdpClient(Port);
    Console.WriteLine($"Listening for UDP messages on port {Port}...");
    Console.WriteLine("Press Ctrl+C to stop.\n");

    while (true)
    {
        var result = await udpClient.ReceiveAsync();
        var message = Encoding.UTF8.GetString(result.Buffer);
        Console.WriteLine($"[{result.RemoteEndPoint}] {message}");
    }
}

async Task RunSender()
{
    using var udpClient = new UdpClient();
    var endpoint = new IPEndPoint(IPAddress.Loopback, Port);

    Console.WriteLine($"Sending UDP messages to 127.0.0.1:{Port}");
    Console.WriteLine("Type a message and press Enter (or 'quit' to exit):\n");

    while (true)
    {
        Console.Write("> ");
        var message = Console.ReadLine();

        if (string.IsNullOrEmpty(message) || message.Equals("quit", StringComparison.OrdinalIgnoreCase))
            break;

        var bytes = Encoding.UTF8.GetBytes(message);
        await udpClient.SendAsync(bytes, bytes.Length, endpoint);
        Console.WriteLine($"  Sent {bytes.Length} bytes.");
    }

    Console.WriteLine("Sender stopped.");
}
