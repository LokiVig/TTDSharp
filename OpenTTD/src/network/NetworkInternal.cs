namespace OpenTTD.Network;

/// <summary>
/// Status of the clients during joining.
/// </summary>
public enum NetworkJoinStatus : byte
{
    Connecting,
    Authorizing,
    Waiting,
    Downloading,
    Processing,
    Registering,

    GettingCompanyInfo,
    End
}

/// <summary>
/// Everything we need to know about a command to be able to execute it.
/// </summary>
public struct CommandPacket
{
    public CompanyID company; // Company that is executing the command
    public uint frame; // The frame in which this packet is executed
    public bool myCmd; // Did the command come from "me"?

    public Commands cmd; // Command being executed
    public StringID errMsg; // String ID of the error message to use
    public CommandCallback callback; // Any callback function executed upon successful completion of the command
    public CommandDataBuffer data; // Command parameters

    public CommandPacket()
    {
        company = InvalidCompany;
        frame = 0;
        myCmd = false;
    }
}

public static partial class Network
{
    public static ServerNetworkGameSocketHandler networkClientSocket;

    public static NetworkJoinStatus networkJoinStatus;
    
    public static byte networkJoinWaiting;
    public static uint networkJoinBytes;
    public static uint networkJoinBytesTotal;

    public static ConnectionType networkServerConnectionType;
    
    public static string networkServerInviteCode;

    public static string networkServerName;

    public static byte networkReconnect;

    public static void QueryServer( string connectionString )
    {

    }

    public static void GetBindAddress( string connectionString )
    {

    }

    public static NetworkGameList NetworkAddServer( string connectionString, bool manually = true, bool neverExpire = false )
    {

    }

    public static void RebuildHostList()
    {
        
    }

    public static void UpdateNetworkGameWindow()
    {

    }

    public static void DistributeCommands()
    {

    }

    public static void ExecuteLocalCommandQueue()
    {

    }

    public static void FreeLocalCommandQueue()
    {

    }

    public static void SyncCommandQueue( NetworkClientSocket cs )
    {

    }

    public static void ReplaceCommandClientId( CommandPacket cp, ClientID clientId )
    {

    }

    public static void ShowNetworkError( StringID errorString )
    {

    }

    public static void TextMessage( NetworkAction action, TextColour colour, bool selfSend, string name, string str = "", long data = 0, string dataPtr = "" )
    {

    }

    public static uint CalculateLag( NetworkClientSocket cs )
    {

    }

    public static StringID GetNetworkErrorMsg( NetworkErrorCode err )
    {

    }

    public static bool MakeClientNameUnique( string newName )
    {

    }

    public static string ParseCompanyFromConnectionString( string connectionString, CompanyID companyId )
    {

    }

    public static NetworkAddress ParseConnectionString( string connectionString, ushort defaultPort )
    {

    }

    public static string NormalizeConnectionString( string connectionString, ushort defaultPort )
    {

    }

    public static void ClientNetworkEmergencySave()
    {

    }
}