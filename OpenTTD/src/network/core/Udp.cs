namespace OpenTTD.Network.Core;

/// <summary>
/// Enum with all the types of UDP packets. The order MUST not be changed.
/// </summary>
public enum PacketUDPType : byte
{
    ClientFindServer, // Queries a game server for game information
    ServerResponse, // Reply of the game server with game information
    End, // Must ALWAYS be on the end of this list!!! (period)
}

/// <summary>
/// Base socket handler for all UDP sockets.
/// </summary>
public class NetworkUDPSocketHandler : NetworkSocketHandler
{

}