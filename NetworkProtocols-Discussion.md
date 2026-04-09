# Network Protocols Discussion — UDP & TCP in C#

## 1. UDP Chat App

A simple C# console app using the UDP protocol with two modes:

- **Receiver** — Listens on UDP port 5000 and prints incoming messages.
- **Sender** — Sends typed messages via UDP to `127.0.0.1:5000`.

**Project:** `UdpApp/Program.cs`

Uses `UdpClient` from `System.Net.Sockets` for connectionless, lightweight communication.

---

## 2. TCP Chat App

A similar C# console app using the TCP protocol:

- **Server** — Starts a `TcpListener` on port 6000, waits for a client, then allows two-way messaging.
- **Client** — Connects to the server and allows two-way messaging.

**Project:** `TcpApp/Program.cs`

### UDP vs TCP Comparison

| Feature         | UDP App                        | TCP App                              |
| --------------- | ------------------------------ | ------------------------------------ |
| **Protocol**    | Connectionless                 | Connection-oriented                  |
| **Reliability** | No delivery guarantee          | Guaranteed, ordered delivery         |
| **Communication** | One-way (sender → receiver)  | Two-way (both can send & receive)    |
| **Port**        | 5000                           | 6000                                 |

---

## 3. UDP Listeners & Port Binding

### Can there be multiple UDP listeners on the same port?

**No.** You cannot bind two `UdpClient` instances to the same port (without special socket options like `SO_REUSEADDR`).

However, in the UDP app, the sender and receiver don't conflict because:

- **Receiver:** `new UdpClient(Port)` — **binds** to port 5000 to listen.
- **Sender:** `new UdpClient()` — creates a client with **no specific port**. The OS assigns a random ephemeral port (e.g., 52314). It then **sends to** port 5000 as the destination.

```
Sender (random port 52314)  ──UDP datagram──►  Receiver (bound to port 5000)
```

If you tried `new UdpClient(Port)` in **both**, the second would throw:
`SocketException: Address already in use`

### TCP Multiple Listeners

In TCP, a single `TcpListener` on a port can **accept multiple client connections** concurrently. Each `AcceptTcpClientAsync()` call returns a dedicated `TcpClient` with its own `NetworkStream`.

---

## 4. TCP — Connection-Oriented Explained

TCP requires a **dedicated session** to be established before any data is exchanged. Like a phone call — you dial, the other side picks up, then you talk.

### The 3-Way Handshake (Connection Setup)

```
Client                         Server
  │                              │
  │──── SYN ────────────────────►│   1. "I want to connect"
  │                              │
  │◄──── SYN-ACK ───────────────│   2. "OK, I acknowledge"
  │                              │
  │──── ACK ────────────────────►│   3. "Great, we're connected"
  │                              │
  │◄════ Data flows both ways ══►│   Connection established!
```

**In the TCP app:**

1. **Server** calls `listener.AcceptTcpClientAsync()` — waits for the handshake to complete.
2. **Client** calls `client.ConnectAsync()` — initiates the 3-way handshake.
3. Once connected, both get a `NetworkStream` — a reliable, ordered, bidirectional pipe.

### What "Connection-Oriented" Guarantees

| Feature              | How it works                                              |
| -------------------- | --------------------------------------------------------- |
| **Reliable delivery**| Every packet is acknowledged; lost packets are retransmitted |
| **Ordered**          | Data arrives in the exact sequence it was sent            |
| **Flow control**     | Sender slows down if receiver can't keep up               |
| **Error detection**  | Checksums catch corrupted data                            |
| **Connection state** | Both sides track the session (connected → data → closed)  |

### Connection Teardown (4-Way)

```
Client                         Server
  │──── FIN ────────────────────►│   "I'm done sending"
  │◄──── ACK ───────────────────│   "OK"
  │◄──── FIN ───────────────────│   "I'm done too"
  │──── ACK ────────────────────►│   "Goodbye"
```

This happens when `client.Close()` is called or the `using` block disposes the `TcpClient`.

### In Short

- **TCP** = phone call (connect → talk → hang up)
- **UDP** = postcard (just send it, hope it arrives)
