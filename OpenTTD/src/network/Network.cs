namespace OpenTTD.Network;

public static partial class Network
{
    public static bool networking; // Are we in networking mode?
    public static bool networkServer; // Network-server is active
    public static bool networkAvailable; // Is network mode available?
    public static bool networkDedicated; // Are we a dedicated server?
    public static bool isNetworkServer; // Does this client want to be a network-server?

    public static ClientID networkOwnClientId; // Our client identifier
    public static ClientID redirectConsoleToClient; // If not invalid, redirect the console output to a client

    public static byte networkReconnect; // Reconnect timeout

    public static StringList networkBindList; // The address to bind on
    public static StringList networkHostList; // The servers we know
    public static StringList networkBanList; // The banned clients

    public static uint frameCounterServer; // The frameCounter of the server, if in network-mode
    public static uint frameCounterMax; // To where we may go with our clients
    public static uint frameCounter; // The current frame
    public static uint lastSyncFrame; // Used in the server to store the last time a sync packet was sent to clients

    public static NetworkAddressList broadcastList; // List of broadcast addresses

    public static uint syncSeed1; // Seed to compare during sync checks
    public static uint syncSeed2; // Second part of the seed
    public static uint syncFrame; // The frame to perform the sync check

    public static bool networkFirstTime; // Whether we've finished joining or not

    public static byte networkClientsConnected = 0; // The amount of clients connected

    public static string GenerateUid( string subject )
    {

    }

    public static bool HasClients()
    {
        return !NetworkClientSocket.Empty();
    }
}