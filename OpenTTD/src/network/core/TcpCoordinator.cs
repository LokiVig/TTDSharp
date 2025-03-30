using System.Net.Sockets;

namespace OpenTTD.Network.Core;

/// <summary>
/// Enum with all types of TCP Game Coordinator packets. The order MUST not be changed.<br/>
/// <br/>
/// GC -> Packets from Game Coordinator to either Client or Server.<br/>
/// Server -> Packets from Server to Game Coordinator.<br/>
/// Client -> Packets from Client to Game Coordinator.<br/>
/// SERCLI -> Packets from either the Server or Client to Game Coordinator.
/// </summary>
public enum PacketCoordinatorType : byte
{
    GC_Error, // Game Coordinator indicates there was an error
    Server_Register, // Server registration
    GC_RegisterAck, // Game Coordinator accepts the registration
    Server_Update, // Server sends and set intervals an update of the server
    Client_Listing, // Client is requesting a listing of all public servers
    GC_Listing, // Game Coordinator returns a listing of all public servers
    Client_Connect, // Client wants to connect to a server based on an invite code
    GC_Connecting, // Game Coordinator informs the client of the token assigned to the connection attempt
    SERCLI_ConnectFailed, // Client / Server tells the Game Coordinator the current connection attempt failed
    GC_ConnectFailed, // Game Coordinator informs Client / Server it has given up on the connection attempt
    Client_Connected, // Client informs the Game Coordinator the connection with the server is established
    GC_DirectConnect, // Game Coordinator tells Client to directly connect to the hostname:port of the server
    GC_STUNRequest, // Game Coordinator tells Client / Server to initiate a STUN request
    SERCLI_STUNResult, // Client / Server informs the Game Coordinator of the result of the STUN request
    GC_STUNConnect, // Game Coordinator tells Client / Server to connect() reusing the STUN local address
    GC_NewGRFLookup, // Game Coordinator informs Client about NewGRF lookup table updates needed for GC_Listing
    GC_TURNConnect, // Game Coordinator tells Client / Server to connect to a specific TURN server
    End, // Must ALWAYS be on the end of this list!! (period)
}

/// <summary>
/// The type of connection the Game Coordinator can detect we have.
/// </summary>
public enum ConnectionType : byte
{
    Unknown, // The Game Coordinator hasn't informed us yet what type of connection we have
    Isolated, // The Game Coordinator failed to find a way to connect to your server, nobody will be able to join
    Direct, // The Game Coordinator can directly connect to your server
    STUN, // The Game Coordinator can connect to your server via a STUN request
    TURN, // The Game Coordinator needs you to connect to a relay
}

/// <summary>
/// The type of error from the Game Coordinator.
/// </summary>
public enum NetworkCoordinatorErrorType : byte
{
    Unknown, // There was an unknown error
    RegistrationFailed, // Your request for registration failed
    InvalidInviteCode, // The invite code given is invalid
    ReuseOfInviteCode, // The invite code is used by another (newer) server
}

/// <summary>
/// Base socket handler for all Game Coordinator TCP sockets.
/// </summary>
public class NetworkCoordinatorSocketHandler : NetworkTCPSocketHandler
{
    /// <summary>
    /// Create a new cs socket handler for a given cs.
    /// </summary>
    /// <param name="s">The socket we are connected with.</param>
    public NetworkCoordinatorSocketHandler( Socket s = null )
        : base( s )
    {
    }

    /// <summary>
    /// Helper for logging receiving invalid packets.
    /// </summary>
    /// <param name="type">The received packet type.</param>
    /// <returns>Always <see langword="false"/>, as it's an error.</returns>
    protected bool ReceiveInvalidPacket( PacketCoordinatorType type )
    {
        // Log
        return false;
    }

    /// <summary>
    /// Game Coordinator indicates there was an error. This can either be a<br/>
    /// permanent error causing the connection to be dropped, or in response<br/>
    /// to a request that is invalid.<br/>
    /// <br/>
    /// <see langword="byte"/> - Type of error (see <see cref="NetworkCoordinatorErrorType"/>).<br/>
    /// <see langword="string"/> - Details of the error.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    /// <returns><see langword="true"/> upon success, otherwise <see langword="false"/>.</returns>
    protected virtual bool ReceiveGCError( Packet p )
    {
        return ReceiveInvalidPacket( PacketCoordinatorType.GC_Error );
    }

    protected virtual bool ReceiveServerError( Packet p )
    {
        return ReceiveInvalidPacket( PacketCoordinatorType.Server_Register );
    }

    /// <summary>
    /// Handle the given packet, i.e. pass it to the right parser<br/>
    /// receive command.
    /// </summary>
    /// <param name="p">The packet to handle.</param>
    /// <returns><see langword="true"/> if we should immediately handle further packets.</returns>
    protected bool HandlePacket( Packet p )
    {
        PacketCoordinatorType type = (PacketCoordinatorType)p.RecvByte();

        switch ( type )
        {
            case PacketCoordinatorType.GC_Error: return ReceiveGCError( p );
            case PacketCoordinatorType.Server_Register: return ReceiveServerError( p );

            default:
                return false;
        }
    }

    /// <summary>
    /// Receive a packet at TCP level.
    /// </summary>
    /// <returns>Whether at least one packet was returned.</returns>
    public bool ReceivePackets()
    {
        // We read only a few of the packets, this allows the GUI to update when
        // a large set of servers is being received, otherwise the interface
        // "hangs" while the game is updating the server-list
        //
        // What arbitrary number to choose is the ultimate question though
        Packet? p;
        const int MAX_PACKETS_TO_RECEIVE = 42;
        int i = MAX_PACKETS_TO_RECEIVE;

        while ( --i != 0 && ( p = ReceivePacket() ) != null )
        {
            bool cont = HandlePacket( p );

            if ( !cont )
            {
                return true;
            }
        }

        return i != MAX_PACKETS_TO_RECEIVE - 1;
    }
}