using System.Net.Sockets;

using OpenTTD.Network.Core;

namespace OpenTTD.Network;

/// <summary>
/// Class for handling the client side of the game connection.
/// </summary>
public class ClientNetworkGameSocketHandler : NetworkGameSocketHandler
{
    private NetworkAuthenticationClientHandler authenticationHandler = null; // The handler for the authentication
    private string connectionString; // Address we're connected to
    private PacketReader savegame = null; // Packet reader for reading the savegame
    private byte token = 0; // The token we need to send back to the server to prove we're the right client

    /// <summary>
    /// Status of the connection with the server.
    /// </summary>
    private enum ServerStatus : byte
    {
        Inactive, // The client is not connected nor active
        Join, // We are trying to join a server
        AuthGame, // Last action was requesting game (server) password
        Encrypted, // The game authentication has completed and from here on the connection to the server is encrypted
        NewGRFsCheck, // Last action was checking NewGRFs
        Authorized, // The client is authorized at the server
        MapWait, // The client is waiting as someone else is downloading the map
        Map, // The client is downloading the map
        Active, // The client is active within the game
        End // Must ALWAYS be on the end of this list!!! (period)
    }

    private ServerStatus status = ServerStatus.Inactive; // Status of the connection with the server

    protected static ClientNetworkGameSocketHandler myClient; // This is us!

    protected void NetworkExecuteLocalCommandQueue()
    {

    }

    protected void NetworkClose( bool closeAdmins )
    {

    }

    protected override NetworkRecvStatus ReceiveServerFull( Packet p )
    {
    }

    protected override NetworkRecvStatus ReceiveServerBanned( Packet p )
    {

    }

    protected override NetworkRecvStatus ReceiveServerError( Packet p )
    {

    }

    protected override NetworkRecvStatus ReceiveServerClientInfo( Packet p )
    {

    }

    protected override NetworkRecvStatus ReceiveServerAuthRequest( Packet p )
    {

    }

    protected override NetworkRecvStatus ReceiveServerEnableEncryption( Packet p )
    {

    }

    protected override NetworkRecvStatus ReceiveServerWelcome( Packet p )
    {

    }

    protected override NetworkRecvStatus ReceiveServerWait( Packet p )
    {

    }

    protected override NetworkRecvStatus ReceiveServerMapBegin( Packet p )
    {

    }

    protected override NetworkRecvStatus ReceiveServerMapSize( Packet p )
    {

    }
}