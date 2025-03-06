using System;
using System.Diagnostics;

using OpenTTD.Network.Core;

namespace OpenTTD.Network;

/// <summary>
/// Some information about a socket, which exists before the actual socket has been created to provide locking and the likes.
/// </summary>
public struct UDPSocket
{
    public string name; // The name of the socket
    public NetworkUDPSocketHandler socket = null; // The actual socket, which may be null when not initialized yet

    public UDPSocket( string name )
    {
        this.name = name;
    }

    public void CloseSocket()
    {
        socket.CloseSocket();
        socket = null;
    }

    public void ReceivePackets()
    {
        socket.ReceivePackets();
    }
}

public static partial class Network
{
    public static bool networkUdpServer; // Is the UDP server started?

    public static ushort networkUdpBroadcast; // Timeout for the UDP broadcasts

    public static UDPSocket udpClient = new UDPSocket( "client" ); // UDP client socket
    public static UDPSocket udpServer = new UDPSocket( "server" ); // UDP server socket

    /// <summary>
    /// Broadcast to all IPs.
    /// </summary>
    public static void NetworkUDPBroadCast( NetworkUDPSocketHandler socket )
    {
        foreach ( NetworkAddress addr in broadcastList )
        {
            Console.WriteLine( $"Broadcasting to {addr.GetHostName()}." );

            Packet p = new Packet( socket, PACKET_UDP_CLIENT_FIND_SERVER );
            socket.SendPacket( p, addr, true, true );
        }
    }

    /// <summary>
    /// Find all servers.
    /// </summary>
    public static void NetworkUDPSearchGame()
    {
        // We are still searching...
        if ( networkUdpBroadcast > 0 )
        {
            return;
        }

        Console.WriteLine( "Searching server" );

        NetworkUDPBroadCast( udpClient.socket );
        networkUdpBroadcast = 300; // Stay searching for 300 ticks
    }

    /// <summary>
    /// Initialize the whole UDP bit.
    /// </summary>
    public static void NetworkUDPInitialize()
    {
        // If not closed, then do it
        if ( udpServer.socket != null )
        {
            NetworkUDPClose();
        }

        Console.WriteLine( "Initializing UDP listeners" );
        Debug.Assert( udpClient.socket == null && udpServer.socket == null );

        udpClient.socket = new ClientNetworkUDPSocketHandler();

        NetworkAddressList server;
        GetBindAddress( server, Settings.settingsClient.network.serverPort );
        udpServer.socket = new ServerNetworkUDPSocketHandler( server );

        networkUdpServer = false;
        networkUdpBroadcast = 0;
    }

    /// <summary>
    /// Start the listening of the UDP server component.
    /// </summary>
    public static void NetworkUDPServerListen()
    {
        networkUdpServer = udpServer.socket.Listen();
    }

    /// <summary>
    /// Close all UDP related stuff.
    /// </summary>
    public static void NetworkUDPClose()
    {
        udpClient.CloseSocket();
        udpServer.CloseSocket();

        networkUdpServer = false;
        networkUdpBroadcast = 0;
        Console.WriteLine( "Closed UDP listeners" );
    }

    /// <summary>
    /// Receive the UDP packets.
    /// </summary>
    public static void NetworkBackgroundUDPLoop()
    {
        if ( networkUdpServer )
        {
            udpServer.ReceivePackets();
        }
        else
        {
            udpClient.ReceivePackets();

            if ( networkUdpBroadcast > 0 )
            {
                networkUdpBroadcast--;
            }
        }
    }
}

/// <summary>
/// Helper class for handling all server side communication.
/// </summary>
public class ServerNetworkUDPSocketHandler : NetworkUDPSocketHandler
{
    protected override void Receive_CLIENT_FIND_SERVER( Packet p, NetworkAddress clientAddr )
    {
        Packet packet = new Packet(this, PACKET_UDP_SERVER_RESPONSE)
    }

    public ServerNetworkUDPSocketHandler( NetworkAddressList addresses ) 
        : base(addresses)
    {
    }
}

/// <summary>
/// Helper class for handling all client side communication.
/// </summary>
public class ClientNetworkUDPSocketHandler : NetworkUDPSocketHandler
{
    protected override void Receive_SERVER_RESPONSE( Packet p, NetworkAddress clientAddr )
    {
        Console.WriteLine( $"Server response from {clientAddr.GetAddressAsString()}." );

        Network.NetworkAddServer( clientAddr.GetAddressAsString( false ), false, true );
    }
}