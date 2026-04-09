# simple-protocols

Simple C# console apps demonstrating UDP and TCP network protocols.

## Projects

### UdpApp
A connectionless chat app using the **UDP** protocol.
- **Receiver** listens on port 5000 for incoming datagrams.
- **Sender** sends messages to `127.0.0.1:5000` from an ephemeral port.

### TcpApp
A connection-oriented chat app using the **TCP** protocol.
- **Server** listens on port 6000 and accepts a client connection.
- **Client** connects to the server for two-way messaging.

## Getting Started

```bash
# Build both projects
dotnet build UdpApp
dotnet build TcpApp
```

### Run UDP App
Open two terminals:
```bash
# Terminal 1 — Receiver
dotnet run --project UdpApp    # Choose option 1

# Terminal 2 — Sender
dotnet run --project UdpApp    # Choose option 2
```

### Run TCP App
Open two terminals:
```bash
# Terminal 1 — Server
dotnet run --project TcpApp    # Choose option 1

# Terminal 2 — Client
dotnet run --project TcpApp    # Choose option 2
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
