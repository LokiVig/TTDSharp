using CommandQueue = System.Collections.Generic.List<OpenTTD.Network.CommandPacket>;

namespace OpenTTD.Network.Core;

/// <summary>
/// Enum with all the types of TCP packets.<br/>
/// For the exact meaning, look at <see cref="NetworkGameSocketHandler"/>.
/// </summary>
public enum PacketGameType : byte
{
    //
    // These first ten packets must remain in this order for backwards and forwards compatibility
    // between clients that are trying to join directly. These packets can be received and / or sent
    // by the server before the server has processed the 'join' packet from the client.
    //

    // Packets sent by socket accepting code without ever constructing a client socket instance
    ServerFull, // The server is full and has no place for you
    ServerBanned, // The server has banned you

    // Packets used by the client to join and an error message when the revision is wrong
    ClientJoin, // The client telling the server it wants to join
    ServerError, // Server sending an error message to the client

    // Unused packet types, formerly used for the pre-game lobby
    ClientUnused, // Unused
    ServerUnused, // Unused

    // A server quitting this game
    ServerGameInfo, // The server is preparing to start a new game
    ServerShutdown, // The server is shutting down

    //
    // Packets after here assume that the client
    // and server are running the same version. As
    // such ordering is unimportant from here on.
    //
    // The following is the remainder of the packets
    // sent as part of authenticating and getting
    // the map and other important data.
    //

    // After the join step, the first perform game authentication and enabling encryption
    ServerAuthRequest, // The server requests the client to authenticate using a number of methods
    ClientAuthResponse, // The client responds to the authentication request
    ServerEnableEncryption, // The server tells that authentication has completed and requests to enable encryption with the keys of the last ClientAuthResponse

    // After the authentication is done, the next step is identification
    ClientIdentify, // Client telling the server the client's name and requested company

    // After the identity step, the next is checking NewGRFs
    ServerCheckNewGRFs, // Server sends NewGRF IDs and MD5 checksums for the client to check
    ClientNewGRFsChecked, // Client acknowledges that it has all the required NewGRFs

    // The server welcomes the authenticated client and sends information of other clients
    ServerWelcome, // Server welcomes you and gives you your ClientID
    ServerClientInfo, // Server sends you information about a client

    // Getting the savegame / map
    ClientGetMap, // Client requests the actual map
    ServerWait, // Server tells the client there are some people waiting for the map as well
    ServerMapBegin, // Server tells the client that it is beginning to send the map
    ServerMapSize, // Server tells the client what the (compressed) size of the map is
    ServerMapData, // Server sends bits of the map to the client
    ServerMapDone, // Server tells the client that it has just sent the last bits of the map to the client
    ClientMapOk, // Client tells the server that it received the whole map

    ServerJoin, // Tells clients that a new client has joined

    //
    // At this moment the client has the map and
    // the client is fully authenticated. Now the
    // normal communication starts.
    //

    // Game progress monitoring
    ServerFrame, // Server tells the client what frame it is in, and thus to where the client may progress
    ClientAck, // The client tells the server which frame it has executed
    ServerSync, // Server tells the client what the random state should be

    // Sending commands around
    ClientCommand, // Client executed a command and sends it to the server
    ServerCommand, // Server distributes a command to (all) the clients

    // Human communication!
    ClientChat, // Client said something that should be distributed
    ServerChat, // Server distributing the message of a client (or itself)
    ServerExternalChat, // Server distributing the message from external source

    // Remote console
    ClientRCon, // Client asks the server to execute some command
    ServerRCon, // Response of the executed command on the server

    // Moving a client
    ClientMove, // A client would like to be moved to another company
    ServerMove, // Server tells everyone that someone has been moved to another company

    // Configuration updates
    ClientSetName, // A client changes its name
    ServerConfigUpdate, // Some network configuration important to the client changed

    // A client quitting
    ClientQuit, // A client tells the server that it's going to quit
    ServerQuit, // A server tells that a client has quit
    ClientError, // A client reports an error to the server
    ServerErrorQuit, // A server tells that a client has hit an error and did quit

    End // Must ALWAYS be at the end of this enum!!! (period)
}

/// <summary>
/// Base socket handler for all TCP sockets
/// </summary>
public class NetworkGameSocketHandler : NetworkTCPSocketHandler
{
    private NetworkClientInfo info = null; // Client info related to this socket
    private bool isPendingDeletion = false; // Whether this socket is pending deletion

    protected NetworkRecvStatus ReceiveInvalidPacket( PacketGameType type )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Notification that the server is full.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveServerFull( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Notification that the client trying to join is banned.
    /// </summary>
    /// <param name="p">The packet that we just received.</param>
    protected virtual NetworkRecvStatus ReceiveServerBanned( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }


    /// <summary>
    /// Try to join the server:<br/>
    /// <see langword="string"/> OpenTTD revision (norev0000 if no revision).<br/>
    /// <see langword="uint"/> NewGRF version (added in 1.2).<br/>
    /// <see langword="string"/> Name of the client (max <see cref="Config.NETWORK_NAME_LENGTH"/>) (removed in 15).<br/>
    /// <see langword="byte"/> ID of the company to play as (1..<see cref="MAX_COMPANIES"/>) (removed in 15).<br/>
    /// <see langword="byte"/> ID of the client's language (removed in 15).<br/>
    /// <see langword="string"/> Client's unique identifier (removed in 1.0).
    /// </summary>
    /// <param name="p">The packet that was just received.</param
    protected virtual NetworkRecvStatus ReceiveClientJoin( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// The client made an error:<br/>
    /// <see langword="byte"/> Error code caused (see <see cref="NetworkErrorCode"/>).
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveServerError( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Request game information.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveClientGameInfo( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Sends information about the game.<br/>
    /// Serialized <see cref="NetworkGameInfo"/>. See GameInfo.cs for details.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveServerGameInfo( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Send information about a client:<br/>
    /// <see langword="uint"/> ID of the client (always unique on a server. 1 = server, 0 is invalid).<br/>
    /// <see langword="byte"/> ID of the company the client is playing as (255 for spectators).<br/>
    /// <see langword="string"/> Name of the client.<br/>
    /// <see langword="string"/> Public key of the client.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveServerClientInfo( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// The client tells the server about the identity of the client:<br/>
    /// <see langword="string"/> Name of the client (max <see cref="Config.NETWORK_CLIENT_NAME_LENGTH"/>).<br/>
    /// <see langword="byte"/> ID of the company to play as (1..<see cref="MAX_COMPANIES"/>, or <see cref="COMPANY_SPECTATOR"/>).
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveClientIdentify( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Indication to the client that it needs to authenticate:<br/>
    /// <see langword="byte"/> The <see cref="NetworkAuthenticationMethod"/> to use.<br/>
    /// 32 * <see langword="byte"/> Public key of the server.<br/>
    /// 24 * <see langword="byte"/> Nonce for the key exchange.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveServerAuthRequest( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Send the response to the authentication request:<br/>
    /// 32 * <see langword="byte"/> Public key of the client.<br/>
    /// 16 * <see langword="byte"/> Message authentication code.<br/>
    ///  8 * <see langword="byte"/> Random message that got encoded and signed.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveClientAuthResponse( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Indication to the client that authentication is complete and encryption has to be used from here on forward.<br/>
    /// The encryption uses the shared keys generated by the last <see cref="AUTH_REQUEST"/> key exchange.<br/>
    /// 24 * <see langword="byte"/> Nonce for encrypted connection.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveServerEnableEncryption( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// The client is joined and ready to receive their map:<br/>
    /// <see langword="uint"/> Own client ID.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveServerWelcome( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Request the map from the server.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveClientGetMap( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Notification that another client is currently receiving the map:<br/>
    /// <see langword="byte"/> Number of clients waiting in front of you.
    /// </summary>
    /// <param name="p">The packet that was just received.</param
    protected virtual NetworkRecvStatus ReceiveServerWait( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Sends that the server will begin with sending the map to the client:<br/>
    /// <see langword="uint"/> Current frame.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveServerMapBegin( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Sends the size of the map to the client.<br/>
    /// <see langword="uint"/> Size of the (compressed) map (in bytes).
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveServerMapSize( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Sends the data of the map to the client:<br/>
    /// Contains a part of the map (until max size of packet).
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveServerMapData( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Sends that all data of the map was sent to the client.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    protected virtual NetworkRecvStatus ReceiveServerMapDone( Packet p )
    {
        return NetworkRecvStatus.Okay;
    }
}