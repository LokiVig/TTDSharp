using System;
using System.Net.Sockets;

using OpenTTD.Network.Core;

namespace OpenTTD.Network;

/// <summary>
/// Class for handling the client side of the TURN connection.
/// </summary>
public class ClientNetworkTurnSocketHandler : NetworkTurnSocketHandler
{
    public TCPConnector connecter = null; // Connecter instance
    public bool connectStarted = false; // Whether we started the connection

    private string token; // Token of this connection
    private byte trackingNumber; // Tracking number of this connection
    private string connectionString; // The connection string of the TURN server we are connecting to

    public ClientNetworkTurnSocketHandler( string token, byte trackingNumber, string connectionString )
    {
        this.token = token;
        this.trackingNumber = trackingNumber;
        this.connectionString = connectionString;
    }

    protected override bool Receive_TURN_ERROR( Packet p )
    {
        Console.WriteLine( "Receive_TURN_ERROR()" );

        ConnectFailure();

        return false;
    }

    protected override bool Receive_TURN_CONNECTED( Packet p )
    {
        Console.WriteLine( "Receive_TURN_CONNECTED()" );

        string hostname = p.RecvString( Config.NETWORK_HOSTNAME_LENGTH );

        // Act like we no longer have a socket, as we are handing it over to the
        // game handler
        Socket gameSock = sock;
        sock = null;

        NetworkAddress address = new NetworkAddress( hostname, Config.NETWORK_DEFAULT_PORT );
        Network.networkCoordinatorClient.ConnectSuccess( token, gameSock, address );

        return false;
    }
    
    public override NetworkRecvStatus CloseConnection( bool error = true )
    {
        NetworkTurnSocketHandler.CloseConnection( error );

        // Also make sure any pending connecter is killed ASAP
        if ( connecter != null )
        {
            connecter.Kill();
            connecter = null;
        }

        return NetworkRecvStatus.Okay;
    }

    /// <summary>
    /// Check whether we received / can send some data from / to the TURN server and<br/>
    /// when that's the case handle it appropriately.
    /// </summary>
    public void SendReceive()
    {
        if ( sock == null )
        {
            return;
        }

        if ( CanSendReceive() )
        {
            ReceivePackets();
        }

        SendPackets();
    }

    /// <summary>
    /// Connect to the TURN server.
    /// </summary>
    public void Connect()
    {
        Console.WriteLine( "Turn.Connect()" );

        connectStarted = true;
        connecter = TCPConnector.Create<NetworkTurnConnecter>( this, connectionString );
    }

    public void ConnectFailure()
    {
        networkCoordinatorClient.ConnectFailure( token, trackingNumber );
    }

    /// <summary>
    /// Prepare a TURN connection.<br/>
    /// Not until you run <see cref="Connect"/> on the resulting instance will it start setting<br/>
    /// up the TURN connection.
    /// </summary>
    /// <param name="token">The token as received from the Game Coordinator.</param>
    /// <param name="trackingNumber">The tracking number as received from the Game Coordinator.</param>
    /// <param name="ticket">The ticket as received from the Game Coordinator.</param>
    /// <param name="connectionString">Connection string of the TURN server.</param>
    /// <returns>The handler for this TURN connection.</returns>
    public static ClientNetworkTurnSocketHandler Turn( string token, byte trackingNumber, string ticket, string connectionString )
    {
        ClientNetworkTurnSocketHandler turnHandler = new ClientNetworkTurnSocketHandler( token, trackingNumber, connectionString );

        Packet p = new Packet( turnHandler.Get(), PACKET_TURN_SERCLI_CONNECT );
        p.SendByte( NETWORK_COORDINATOR_VERSION );
        p.SendString( ticket );
    }
}

/// <summary>
/// Connect to the TURN server.
/// </summary>
public class NetworkTurnConnecter : TCPConnector
{
    private ClientNetworkTurnSocketHandler handler;

    /// <summary>
    /// Initialzie the connecting.
    /// </summary>
    /// <param name="connectionString">The address of the TURN server.</param>
    public NetworkTurnConnecter( ClientNetworkTurnSocketHandler handler, string connectionString ) : base(connectionString, Config.NETWORK_TURN_SERVER_PORT)
    {
        this.handler = handler;
    }

    public override void OnFailure()
    {
        Console.WriteLine( "Turn.OnFailure()" );

        handler.connecter = null;
        handler.ConnectFailure();
    }

    public override void OnConnect( Socket s )
    {
        Console.WriteLine( "Turn.OnConnect()" );

        handler.connecter = null;
        handler.sock = s;
    }
}