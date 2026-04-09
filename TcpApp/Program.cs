using System.Net;
using System.Net.Sockets;
using System.Text;

const int Port = 6000;

Console.WriteLine("=== TCP Chat App ===");
Console.WriteLine("1. Start as Server");
Console.WriteLine("2. Start as Client");
Console.Write("Choose mode: ");

var choice = Console.ReadLine();

if (choice == "1")
    await RunServer();
else if (choice == "2")
    await RunClient();
else
    Console.WriteLine("Invalid choice.");

async Task RunServer()
{
    var listener = new TcpListener(IPAddress.Loopback, Port);
    listener.Start();
    Console.WriteLine($"Server listening on 127.0.0.1:{Port}...");
    Console.WriteLine("Waiting for a client to connect...\n");

    using var client = await listener.AcceptTcpClientAsync();
    Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

    await using var stream = client.GetStream();
    var buffer = new byte[1024];

    // Start a background task to send messages
    var sendTask = Task.Run(async () =>
    {
        while (client.Connected)
        {
            Console.Write("[You] ");
            var message = Console.ReadLine();

            if (string.IsNullOrEmpty(message) || message.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                client.Close();
                break;
            }

            var bytes = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(bytes);
        }
    });

    // Receive messages
    try
    {
        while (client.Connected)
        {
            var bytesRead = await stream.ReadAsync(buffer);
            if (bytesRead == 0) break;

            var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"\r[Client] {message}");
            Console.Write("[You] ");
        }
    }
    catch (Exception) { /* connection closed */ }

    Console.WriteLine("\nConnection closed.");
    listener.Stop();
}

async Task RunClient()
{
    using var client = new TcpClient();
    Console.WriteLine($"Connecting to 127.0.0.1:{Port}...");

    try
    {
        await client.ConnectAsync(IPAddress.Loopback, Port);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not connect: {ex.Message}");
        return;
    }

    Console.WriteLine("Connected to server!\n");

    await using var stream = client.GetStream();
    var buffer = new byte[1024];

    // Start a background task to send messages
    var sendTask = Task.Run(async () =>
    {
        while (client.Connected)
        {
            Console.Write("[You] ");
            var message = Console.ReadLine();

            if (string.IsNullOrEmpty(message) || message.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                client.Close();
                break;
            }

            var bytes = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(bytes);
        }
    });

    // Receive messages
    try
    {
        while (client.Connected)
        {
            var bytesRead = await stream.ReadAsync(buffer);
            if (bytesRead == 0) break;

            var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"\r[Server] {message}");
            Console.Write("[You] ");
        }
    }
    catch (Exception) { /* connection closed */ }

    Console.WriteLine("\nConnection closed.");
}
