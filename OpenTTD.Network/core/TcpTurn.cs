using System;
using System.Net.Sockets;

namespace OpenTTD.Network.Core;

/// <summary>
/// Enum with all types of TCP TURN packets. The order MUST not be changed.
/// </summary>
public enum PacketTurnType : byte
{
    TurnError, // TURN server is unable to relay
    SerCliConnect, // Client or server is connecting to the TURN server
    TurnConnected, // TURN server indicates the socket is now being relayed
    End // Must ALWAYS be on the end of this list!!! (period)
}

/// <summary>
/// Base socket handler for all TURN TCP sockets.
/// </summary>
public class NetworkTurnSocketHandler : NetworkTCPSocketHandler
{
    /// <summary>
    /// Create a new cs socket handler for a given cs.
    /// </summary>
    /// <param name="s">The socket we are connected with.</param>
    public NetworkTurnSocketHandler( Socket s = null ) : base( s ) { }

    protected bool ReceiveInvalidPacket( PacketTurnType type )
    {
        Console.WriteLine( $"[TCP/TURN] Received illegal packet type {type}" );
        return false;
    }

    /// <summary>
    /// TURN server was unable to connect the client or server based on the<br/>
    /// token. Most likely cause is an invalid token or the other side that<br/>
    /// hasn't connected in a reasonable amount of time.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    /// <returns><see langword="true"/> upon success, otherwise <see langword="false"/>.</returns>
    protected virtual bool Receive_TURN_ERROR( Packet p )
    {
        return ReceiveInvalidPacket( PacketTurnType.TurnError );
    }

    /// <summary>
    /// Client or servers wants to connect to the TURN server (on request by<br/>
    /// the Game Coordinator).<br/>
    /// <br/>
    /// <see langword="byte"/> - Game Coordinator protocol version.<br/>
    /// <see langword="string"/> - Token to track the current TURN request.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    /// <returns><see langword="true"/> upon success, otherwise <see langword="false"/>.</returns>
    protected virtual bool Receive_SERCLI_CONNECT( Packet p )
    {
        return ReceiveInvalidPacket( PacketTurnType.SerCliConnect );
    }

    /// <summary>
    /// TURN server has connected client and server together and will now relay<br/>
    /// all packets to each other. No further TURN packets should be sent over<br/>
    /// this socket, and the socket should be handed over to the game protocol.<br/>
    /// <br/>
    /// <see langword="string"/> - Hostname of the peer. This can be used to check if a client is not banned, etc.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    /// <returns><see langword="true"/> upon success, otherwise <see langword="false"/>.</returns>
    protected virtual bool Receive_TURN_CONNECTED( Packet p )
    {
        return ReceiveInvalidPacket( PacketTurnType.TurnConnected );
    }

    /// <summary>
    /// Handle the given packet, i.e. pass it to the right<br/>
    /// parser receive command.
    /// </summary>
    /// <param name="p">The packet to handle.</param>
    /// <returns><see langword="true"/> if we should immediately handle further packets, <see langword="false"/> otherwise.</returns>
    protected bool HandlePacket( Packet p )
    {
        PacketTurnType type = (PacketTurnType)p.RecvByte();

        switch ( type )
        {
            case PacketTurnType.TurnError:
                return Receive_TURN_ERROR( p );

            case PacketTurnType.SerCliConnect:
                return Receive_SERCLI_CONNECT( p );

            case PacketTurnType.TurnConnected:
                return Receive_TURN_CONNECTED( p );

            default:
                Console.WriteLine( $"[TCP/TURN] Received invalid packet type {type}" );
                return false;
        }
    }

    /// <summary>
    /// Receive a packet at TCP level.
    /// </summary>
    /// <returns>Whether at least one packet was received.</returns>
    public bool ReceivePackets()
    {
        Packet p;
        int maxPacketsToRecieve = 4;
        int i = maxPacketsToRecieve;

        while ( i-- != 0 && ( p = ReceivePacket() ) != null )
        {
            bool cont = HandlePacket( p );

            if ( !cont )
            {
                return true;
            }
        }

        return i != maxPacketsToRecieve - 1;
    }
}