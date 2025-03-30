using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace OpenTTD.Network.Core;

/// <summary>
/// The states of sending the packets.
/// </summary>
public enum SendPacketsState : byte
{
    Closed, // The connection got closed
    NoneSent, // The buffer is still full, so no (parts of) packets could be sent
    PartlySent, // The packets are partly sent; there are more packets to be sent in the queue
    AllSent, // All packets in the queue are sent
}

/// <summary>
/// Base socket handler for all TCP sockets.
/// </summary>
public class NetworkTCPSocketHandler : NetworkSocketHandler
{
    public Socket sock = null; // The socket currently connected to
    public bool writable = false; // Can we write to this socket?

    private List<Packet> packetQueue; // Packets that are awaiting delivery
    private Packet packetRecv = default; // Partially received packet

    /// <summary>
    /// Construct a socket handler for a TCP connection.
    /// </summary>
    /// <param name="s">The just opened TCP connection.</param>
    public NetworkTCPSocketHandler( Socket s = null )
    {
        sock = s;
    }

    private void EmptyPacketQueue()
    {

    }

    /// <summary>
    /// Whether this socket is currently bound to a socket.
    /// </summary>
    /// <returns><see langword="true"/> when the socket is bound, <see langword="false"/> otherwise.</returns>
    public bool IsConnected()
    {
        return sock != null;
    }

    public virtual NetworkRecvStatus CloseConnection( bool error = true )
    {

    }

    public void CloseSocket()
    {

    }

    public virtual void SendPacket( Packet packet )
    {

    }

    public SendPacketsState SendPackets( bool closingDown = false )
    {

    }

    public virtual Packet ReceivePacket()
    {

    }

    public bool CanSendReceive()
    {

    }

    /// <summary>
    /// Whether there is something pending in the send queue.
    /// </summary>
    /// <returns><see langword="true"/> when something is pending in the send queue.</returns>
    public bool HasSendQueue()
    {
        return packetQueue.Count != 0;
    }
}

/// <summary>
/// "Helper" class for creating TCP connections in a non-blocking manner.
/// </summary>
public class TCPConnector
{
    /// <summary>
    /// The current status of the connecter.<br/>
    /// <br/>
    /// We track the status like this to ensure everything is executed from the<br/>
    /// game-thread, and not at another random time where we might not have the<br/>
    /// lock on the game-state.
    /// </summary>
    private enum Status : byte
    {
        Init, // TCPConnecter is created but resolving hasn't started
        Resolving, // The hsotname is being resolved (threaded)
        Failure, // Resolving failed
        Connecting, // We are currently connecting
        Connected, // The connection is established
    }

    private Thread resolveThread; // Thread used during resolving
    private Status status = Status.Init; // The current status of the connecter
    private bool killed = false; // Whether this connecter is marked as killed

    private AddrInfo ai = null; // GetAddrInfo() allocated linked-list of resolved addresses
    private List<AddrInfo> addresses; // Addresses we can connect to
    private Dictionary<Socket, NetworkAddress> sockToAddress; // Mapping of a socket to the real address it is connecting to, used for DEBUG statements
    private ulong currentAddress = 0; // Current index in addresses we are trying

    private List<Socket> sockets; // Pending connect() attempts
}