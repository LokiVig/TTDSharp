using System;
using System.Runtime.InteropServices;

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
    protected NetworkAddressList bind; // The address to bind to
    protected SocketList sockets; // The opened sockets

    /// <summary>
    /// Helper for logging receiving invalid packets.
    /// </summary>
    /// <param name="type">The received packet type.</param>
    /// <param name="clientAddr">The address we received the packet from.</param>
    protected void ReceiveInvalidPacket( PacketUDPType type, NetworkAddress clientAddr )
    {
        Console.WriteLine( $"[UDP] Received packet type {type} on wrong port from {clientAddr.GetAddressAsString()}" );
    }

    /// <summary>
    /// Queries to the server for information about the game.
    /// </summary>
    /// <param name="p">The received packet.</param>
    /// <param name="clientAddr">The origin of the packet.</param>
    protected virtual void Receive_CLIENT_FIND_SERVER( Packet p, NetworkAddress clientAddr )
    {
        ReceiveInvalidPacket( PacketUDPType.ClientFindServer, clientAddr );
    }

    /// <summary>
    /// Response to a query letting the client know we are here.
    /// </summary>
    /// <param name="p">The received packet.</param>
    /// <param name="clientAddr">The origin of the packet.</param>
    protected virtual void Receive_SERVER_RESPONSE( Packet p, NetworkAddress clientAddr )
    {
        ReceiveInvalidPacket( PacketUDPType.ServerResponse, clientAddr );
    }

    /// <summary>
    /// Handle an incoming packet by sending it to the correct function.
    /// </summary>
    /// <param name="p">The received packet.</param>
    /// <param name="clientAddr">The sender of the packet.</param>
    protected void HandleUDPPacket( Packet p, NetworkAddress clientAddr )
    {
        PacketUDPType type;

        // New packet == new client, which has not quit yet
        Reopen();

        type = (PacketUDPType)p.RecvUShort();

        switch ( HasClientQuit() ? PacketUDPType.End : type )
        {
            case PacketUDPType.ClientFindServer:
                Receive_CLIENT_FIND_SERVER( p, clientAddr );
                break;

            case PacketUDPType.ServerResponse:
                Receive_SERVER_RESPONSE( p, clientAddr );
                break;

            default:
                if ( HasClientQuit() )
                {
                    Console.WriteLine( $"[UDP] Received invalid packet {type} from {clientAddr.GetAddressAsString()}" );
                }
                else
                {
                    Console.WriteLine( $"[UDP] Received illegal packet from {clientAddr.GetAddressAsString()}" );
                }
                break;
        }
    }

    public NetworkUDPSocketHandler( NetworkAddressList bind = null )
    {
        if ( bind != null )
        {
            foreach ( NetworkAddress addr in bind )
            {
                this.bind.Add( addr );
            }
        }
        else
        {
            // As an empty hostname and port 0 don't go well when
            // resolving it we need to add an address for each of
            // the address families we support
            bind.Add( "", 0, AF_INET );
            bind.Add( "", 0, AF_INET6 );
        }
    }

    ~NetworkUDPSocketHandler()
    {
        CloseSocket();
    }

    /// <summary>
    /// Start listening on the given host and port.
    /// </summary>
    /// <returns><see langword="true"/> if at least one port is listening.</returns>
    public bool Listen()
    {
        // Make sure the socket is closed
        CloseSocket();

        foreach ( NetworkAddress addr in bind )
        {
            addr.Listen( SOCK_DGRAM, sockets );
        }

        return !sockets.Empty();
    }

    /// <summary>
    /// Close the actual UDP socket.
    /// </summary>
    public void CloseSocket()
    {
        foreach ( var s in sockets )
        {
            closesocket( s.first );
        }

        sockets.Clear();
    }

    /// <summary>
    /// Send a packet over UDP.
    /// </summary>
    /// <param name="p">The packet to send.</param>
    /// <param name="recv">The receiver (target) of the packet.</param>
    /// <param name="all">Send the packet to all sockets that can send it.</param>
    /// <param name="broadcast">Whether to send a broadcast message.</param>
    public void SendPacket( Packet p, NetworkAddress recv, bool all = false, bool broadcast = false )
    {
        if ( sockets.Empty() )
        {
            Listen();
        }

        foreach ( var s in sockets )
        {
            // Make a local copy of because if we resolve it we cannot
            // easily unresolve it so we can resolve it later again
            NetworkAddress send = new NetworkAddress( recv );

            // Not the same type
            if ( !send.IsFamily( s.second.GetAddress().ssFamily ) )
            {
                continue;
            }

            p.PrepareToSend();

            if ( broadcast )
            {
                // Enable broadcast
                ulong val = 1;

                if ( setsockopt( s.first, SOL_SOCKET, SO_BROADCAST, (sbyte)val, Marshal.SizeOf( val ) ) < 0 )
                {
                    Console.WriteLine( $"Setting broadcast mode failed: {NetworkError.GetLast().AsString()}" );
                }
            }

            // Send the buffer
            dynamic res = p.TransferOut<int>( sendto, s.first );
            Console.WriteLine( $"sendto({send.GetAddressAsString()})" );

            // Check for any errors, but ignore it otherwise
            if ( res == -1 )
            {
                Console.WriteLine( $"sendto({send.GetAddressAsString()}) failed: {NetworkError.GetLast().AsString()}" );
            }

            if ( !all )
            {
                break;
            }
        }
    }

    /// <summary>
    /// Receive a packet at UDP level.
    /// </summary>
    public void ReceivePackets()
    {
        foreach ( var s in sockets )
        {
            for ( int i = 0; i < 1000; i++ ) // Don't infinitely loop when DoSing with UDP
            {
                // This limit is UDP_MTU, but also allocate that much as we need to read the whole packet in one go.
                Packet p = new Packet( this, UDP_MTU, UDP_MTU );

                // Try to receive anything
                SetNonBlocking( s.first ); // Some OSes seem to lose the non-blocking status of the socket
                dynamic nBytes = p.TransferIn<int>( recvfrom, s.first );

                // Did we get the bytes for the base header of the packet?
                if ( nBytes <= 0 ) // No data, i.e. no packet
                {
                    break;
                }

                if ( nBytes <= 2 ) // Invalid data; try next packet
                {
                    continue;
                }

                NetworkAddress address = new NetworkAddress( clientAddr, clientLen );

                // If the size doesn't match the packet must be corrupted
                // Otherwise it'll be marked as corrupted later on
                if ( !p.ParsePacketSize() || nBytes != p.Size() )
                {
                    Console.WriteLine( $"Received a packet with mismatching size from {address.GetAddressAsString()}" );
                    continue;
                }

                if ( !p.PrepareToRead() )
                {
                    Console.WriteLine( $"Invalid packet received (too small / decryption error)" );
                    continue;
                }

                // Handle the packet
                HandleUDPPacket( p, address );
            }
        }
    }
}