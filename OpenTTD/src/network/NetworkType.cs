using System.Collections.Generic;

namespace OpenTTD.Network;

public static partial class Network
{
    public const uint MAX_CLIENTS = 255; // How many clients can we have?
}

/// <summary>
/// <see cref="VehicleType"/>s in order they are sent in info packets.
/// </summary>
public enum NetworkVehicleType : byte
{
    Train = 0,
    Lorry,
    Bus,
    Plane,
    Ship,

    End
}

/// <summary>
/// Game type the server can be using.<br/>
/// Used on the network protocol to communicate with Game Coordinator.
/// </summary>
public enum ServerGameType : byte
{
    Local = 0,
    Public,
    InviteOnly
}

/// <summary>
/// 'Unique' identifier to be given to clients.
/// </summary>
public enum ClientID : uint
{
    Invalid = 0, // Client is not part of anything
    Server = 1, // Servers always have this ID
    First = 2, // The first client ID
}

public struct NetworkCompanyStats
{
    public ushort[] numVehicles = new ushort[(int)NetworkVehicleType.End]; // How many vehicles are there of this type?
    public ushort[] numStations = new ushort[(int)NetworkVehicleType.End]; // How many stations are there of this type?
    public bool ai; // Is this company an AI?

    public NetworkCompanyStats() { }
}

/// <summary>
/// Destination of our chat messages.<br/>
/// WARNING: The values of the enum items are part of the admin network API. Only append at the end.
/// </summary>
public enum DestType : byte
{
    Broadcast, // Send message / notice to all clients (All)
    Team, // Send message / notice to everyone playing the same company (Team)
    Client // Send message / notice to only a certain client (Private)
}

/// <summary>
/// Actions that can be used for <see cref="NetworkTextMessage"/>.<br/>
/// WARNING: The values of the enum items are part of the admin network API. Only append at the end.
/// </summary>
public enum NetworkAction : byte
{
    Join,
    Leave,
    ServerMessage,
    Chat,
    ChatCompany,
    ChatClient,
    GiveMoney,
    NameChange,
    CompanySpectator,
    CompanyJoin,
    CompanyNew,
    Kicked,
    ExternalChat
}

/// <summary>
/// The error codes we send around in the protocols.<br/>
/// WARNING: The values of the enum items are part of the admin network API. Only append at the end.
/// </summary>
public enum NetworkErrorCode : byte
{
    General, // Try to use this one like never

    // Signals from clients
    Desync,
    SaveGameFailed,
    ConnectionLost,
    IllegalPacket,
    NewGRFMismatch,

    // Signals from servers
    NotAuthorized,
    NotExpected,
    WrongRevision,
    NameInUse,
    WrongPassword,
    CompanyMismatch, // Happens in CLIENT_COMMAND
    Kicked,
    Cheater,
    Full,
    TooManyCommands,
    TimeoutPassword,
    TimeoutComputer,
    TimeoutMap,
    TimeoutJoin,
    InvalidClientName,
    NotOnAllowList,
    NoAuthenticationMethodAvailable,

    End
}

/// <summary>
/// Simple helper to (more easily) manage authorized keys.<br/>
/// <br/>
/// The authorized keys are hexadecimal representations of their binary form.<br/>
/// The authorized keys are case insensitive.
/// </summary>
public class NetworkAuthorizedKeys : List<string>
{
    public bool Contains( string key )
    {

    }

    public bool Add( string key )
    {

    }

    public bool Remove( string key )
    {

    }
}