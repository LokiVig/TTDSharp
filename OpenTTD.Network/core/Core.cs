namespace OpenTTD.Network.Core;

/// <summary>
/// Status of a network client; reasons why a client has quit.
/// </summary>
public enum NetworkRecvStatus : byte
{
    Okay, // Everything is okay
    Desync, // A desync did occur
    NewGRFMismatch, // We did not have the required NewGRFs
    Savegame, // Something went wrong (down)loading the savegame
    ClientQuit, // The connection is lost gracefully, other clients are already informed of this leaving client
    MalformedPacket, // We apparently sent a malformed packet
    ServerError, // The server told us we made an error
    ServerFull, // The server is full
    ServerBanned, // The server has banned us
    CloseQuery, // Done querying the server
    ConnectionLost // The connection is lost unexpectedly
}

/// <summary>
/// SocketHandler for all network sockets in OpenTTD.
/// </summary>
public class NetworkSocketHandler
{
    private bool hasQuit = false;

    protected NetworkEncryptionHandler receiveEncryptionHandler = null; // The handler for decrypting received packets
    protected NetworkEncryptionHandler sendEncryptionHandler = null; // The handler for encrypting sent packets

    /// <summary>
    /// Create a new unbound socket.
    /// </summary>
    public NetworkSocketHandler()
    {
        
    }

    /// <summary>
    /// Mark the connection as closed.<br/>
    /// <br/>
    /// This doesn't mean the actual connection is closed, but just that we<br/>
    /// act like it is. This is useful for UDP, which doesn't normally close<br/>
    /// a socket, but its handler might need to pretend it does.
    /// </summary>
    public void MarkClosed()
    {
        hasQuit = true;
    }

    /// <summary>
    /// Whether the current client connected to the socket has quit.<br/>
    /// In the case of UDP, for example, once a client quits (send bad<br/>
    /// data), the socket is not closed; only the packet is dropped.
    /// </summary>
    /// <returns><see langword="true"/> when the current client has quit, <see langword="false"/> otherwise.</returns>
    public bool HasClientQuit()
    {
        return hasQuit;
    }

    /// <summary>
    /// Reopen the socket so we can send / receive stuff again.
    /// </summary>
    public void Reopen()
    {
        hasQuit = false;
    }
}

public static partial class Core
{
    /// <summary>
    /// Initializes the network core (as that is needed for some platforms)
    /// </summary>
    /// <returns><see langword="true"/> if the core has been initialized, <see langword="false"/> otherwise.</returns>
    public static bool Initialize()
    {
        return true;
    }

    /// <summary>
    /// Shuts down the network core (as that is needed for some platforms)
    /// </summary>
    public static void Shutdown()
    {

    }
}