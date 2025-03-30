global using ContentID = uint;

using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace OpenTTD.Network.Core;

/// <summary>
/// The values in the enum are important; they are used as database 'keys'.
/// </summary>
public enum ContentType : byte
{
    Begin = 1, // Helper to mark the begin of the types
    BaseGraphics = 1, // The content consists of base graphics
    NewGRF = 2, // The content consists of a NewGRF
    AI = 3, // The content consists of an AI
    AILibrary = 4, // The content consists of an AI library
    Scenario = 5, // The content consists of a scenario
    Heightmap = 6, // The content consists of a heightmap
    BaseSounds = 7, // The content consists of base sounds
    BaseMusic = 8, // The content consists of base music
    Game = 9, // The content consists of a game script
    GameLibrary = 10, // The content consists of a GS library   
    End, // Helper to mark the end of the types
    Invalid = 0xFF // Invalid / uninitialized content
}

/// <summary>
/// Enum with all the types of TCP content packets. The order MUST not be changed.
/// </summary>
public enum PacketContentType : byte
{
    Client_InfoList, // Queries the content server for a list of info of a given content type
    Client_InfoID, // Queries the content server for information about a list of internal IDs
    Client_InfoExtID, // Queries the content server for information about a list of external IDs
    Client_InfoExtID_MD5, // Queries the content server for information about a list of external IDs and MD5
    Server_Info, // Reply of content server with information about content
    Client_Content, // Request a content file given an internal ID
    Server_Content, // Reply with the content of the given ID
    End // Must ALWAYS be on the end of this list!! (period)
}

/// <summary>
/// Container for all the important information about a piece of content.
/// </summary>
public struct ContentInfo
{
    /// <summary>
    /// The state the content can be in.
    /// </summary>
    public enum State : byte
    {
        Unselected, // The content has not been selected
        Selected, // The content has been manually selected
        AutoSelected, // The content has been selected as a dependency
        AlreadyHere, // The content is already at the client side
        DoesNotExist, // The content does not exist in the content system
        Invalid // The content's invalid
    }

    public ContentType type = ContentType.Invalid; // Type of content
    public ContentID id = INVALID_CONTENT_ID; // Unique (server side) ID for the content

    public uint filesize = 0; // Size of the file

    public string filename; // Filename (for the .tar.gz; only valid on download)
    public string name; // Name of the content
    public string version; // Version of the content
    public string url; // URL related to the content
    public string description; // Description of the content

    public uint uniqueId = 0; // Unique ID; either GRF ID or shortname

    public MD5Hash md5Sum; // The MD5 checksum

    public List<ContentID> dependencies; // The dependencies (unique server side IDs)
    public List<string> tags; // Tags associated with the content

    public State state = State.Unselected; // Whether the content info is selected (for download)

    public bool upgrade = false; // This item is an upgrade

    public ContentInfo() { }

    /// <summary>
    /// Is the state either selected or autoselected?
    /// </summary>
    /// <returns><see langword="true"/> if that's the case.</returns>
    public bool IsSelected()
    {
        switch ( state )
        {
            case State.Selected:
            case State.AutoSelected:
            case State.AlreadyHere:
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Is the information from this content info valid?
    /// </summary>
    /// <returns><see langword="true"/> if it is valid.</returns>
    public bool IsValid()
    {
        return state < State.Invalid && type >= ContentType.Begin && type < ContentType.End;
    }

    /// <summary>
    /// Search a textfile file next to this file in the content list.
    /// </summary>
    /// <param name="type">The type of the textfile to search for.</param>
    /// <returns>The filename for the textfile.</returns>
    public string GetTextFile( TextfileType type )
    {
        if ( state == State.Invalid )
        {
            return string.Empty;
        }

        string tmp = string.Empty;

        switch ( type )
        {
            default:
                throw new NotImplementedException( "NOT_REACHED()" );

            case ContentType.AI:
                tmp = AI.GetScannerInfo().FindMainScript( this, true );
                break;

            case ContentType.AILibrary:
                tmp = AI.GetScannerLibrary().FindMainScript( this, true );
                break;

            case ContentType.Game:
                tmp = Game.GetScannerInfo().FindMainScript( this, true );
                break;

            case ContentType.GameLibrary:
                tmp = Game.GetScannerLibrary().FindMainScript( this, true );
                break;

            case ContentType.NewGRF:
                GRFConfig gc = FindGRConfig( uniqueId, FGCM_EXACT, md5Sum );
                tmp = gc != null ? gc.filename : string.Empty;
                break;

            case ContentType.BaseGraphics:
                tmp = TryGetBaseSetFile( this, true, BaseGraphics.GetAvailableSets() );
                break;

            case ContentType.BaseSounds:
                tmp = TryGetBaseSetFile( this, true, BaseSounds.GetAvailableSets() );
                break;

            case ContentType.BaseMusic:
                tmp = TryGetBaseSetFile( this, true, BaseMusic.GetAvailableSets() );
                break;

            case ContentType.Scenario:
            case ContentType.Heightmap:
                tmp = FindScenario( this, true );
                break;
        }

        if ( tmp == string.Empty )
        {
            return string.Empty;
        }

        return GetTextFile( type, NetworkCore.GetContentInfoSubDir( type ), tmp );
    }
}

/// <summary>
/// Base socket handler for all Content TCP sockets.
/// </summary>
public class NetworkContentSocketHandler : NetworkTCPSocketHandler
{
    /// <summary>
    /// Create a new cs socket handler for a given cs.
    /// </summary>
    /// <param name="s">The socket we are connected with.</param>
    public NetworkContentSocketHandler( Socket s )
        : base( s )
    {
    }

    /// <summary>
    /// Helper for logging receiving invalid packets.
    /// </summary>
    /// <param name="type">The received packet type.</param>
    /// <returns>Always <see langword="false"/>, as it's an error.</returns>
    protected bool ReceiveInvalidPacket( PacketContentType type )
    {
        Console.WriteLine( $"[TCP/Content] Received illegal packet type {type}" );
        return false;
    }

    /// <summary>
    /// Client requesting a list of content info:<br/>
    /// <see langword="byte"/> - Type<br/>
    /// <see langword="uint"/> - OpenTTD version (or <c>0xFFFFFFFF</c> if using a list)<br/>
    /// <br/>
    /// Only if the above value is <c>0xFFFFFFFF</c>:<br/>
    /// <see langword="byte"/> - Count<br/>
    /// <see langword="string"/> - Branch-name ("vanilla" for upstream OpenTTD)<br/>
    /// <see langword="string"/> - Release version (like "12:0")
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    /// <returns><see langword="true"/> upon success, otherwise <see langword="false"/>.</returns>
    protected virtual bool ReceiveClientInfoList( Packet p )
    {
        return ReceiveInvalidPacket( PacketContentType.Client_InfoList );
    }

    /// <summary>
    /// Client requesting a list of content info:<br/>
    /// <see langword="ushort"/> - Count of IDs<br/>
    /// <see langword="uint"/> - ID (count times)
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    /// <returns><see langword="true"/> upon success, otherwise <see langword="false"/>.</returns>
    protected virtual bool ReceiveClientInfoID( Packet p )
    {
        return ReceiveInvalidPacket( PacketContentType.Client_InfoID );
    }

    /// <summary>
    /// Client requesting a list of content info based on an external<br/>
    /// 'unique' ID; GRF ID for NewGRFs, shortname and for base<br/>
    /// graphics and AIs.<br/>
    /// Scenarios and AI libraries are not supported.<br/>
    /// <br/>
    /// <see langword="byte"/> - Count of requests<br/>
    /// For each request:<br/>
    /// <see langword="byte"/> - Type<br/>
    /// <see langword="uint"/> - Unique ID
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    /// <returns><see langword="true"/> upon success, otherwise <see langword="false"/>.</returns>
    protected virtual bool ReceiveClientInfoExtID( Packet p )
    {
        return ReceiveInvalidPacket( PacketContentType.Client_InfoExtID );
    }

    /// <summary>
    /// Client requesting a list of content info based on an external<br/>
    /// 'unique' ID; GRF ID + MD5 checksum for NewGRFs, shortname and<br/>
    /// xor-ed MD5 checksum for base graphics and AIs.<br/>
    /// Scenarios and AI libraries are not supported.<br/>
    /// <br/>
    /// <see langword="byte"/> - Count of requests<br/>
    /// For each request:<br/>
    /// <see langword="byte"/> - Type<br/>
    /// <see langword="uint"/> - Unique ID<br/>
    /// <see langword="byte"/> - MD5
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    /// <returns><see langword="true"/> upon success, otherwise <see langword="false"/>.</returns>
    protected virtual bool ReceiveClientInfoExtIDMD5( Packet p )
    {
        return ReceiveInvalidPacket( PacketContentType.Client_InfoExtID_MD5 );
    }

    /// <summary>
    /// Server sending a list of content info:<br/>
    /// <see langword="byte"/> - Type (invalid ID == does not exist)<br/>
    /// <see langword="uint"/> - ID<br/>
    /// <see langword="uint"/> - Filesize<br/>
    /// <see langword="string"/> - Name (max 32 characters)<br/>
    /// <see langword="string"/> - Version (max 16 characters)<br/>
    /// <see langword="uint"/> - Unique ID<br/>
    /// <see langword="byte"/> - MD5Sum (16 bytes)<br/>
    /// <see langword="byte"/> - Dependency count<br/>
    /// <see langword="uint"/> - Unique ID of dependency (dependency count times)<br/>
    /// <see langword="byte"/> - Tag count<br/>
    /// <see langword="string"/> - Tag (max 32 characters for tag count times)
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    /// <returns><see langword="true"/> upon success, otherwise <see langword="false"/>.</returns>
    protected virtual bool ReceiveServerInfo( Packet p )
    {
        return ReceiveInvalidPacket( PacketContentType.Server_Info );
    }

    /// <summary>
    /// Client requesting the actual content:<br/>
    /// <see langword="ushort"/> - Count of unique IDs<br/>
    /// <see langword="uint"/> - Unique ID (count times)
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    /// <returns><see langword="true"/> upon success, otherwise <see langword="false"/>.</returns>
    protected virtual bool ReceiveClientContent( Packet p )
    {
        return ReceiveInvalidPacket( PacketContentType.Client_Content );
    }

    /// <summary>
    /// Server sending list of content info:<br/>
    /// <see langword="uint"/> - Unique ID<br/>
    /// <see langword="uint"/> - Filesize (0 == does not exist)<br/>
    /// <see langword="string"/> - Filename (max 48 characters)<br/>
    /// <br/>
    /// After this initial packet, packets with the actual data are sent using<br/>
    /// the same packet type.
    /// </summary>
    /// <param name="p">The packet that was just received.</param>
    /// <returns><see langword="true"/> upon success, otherwise <see langword="false"/>.</returns>
    protected virtual bool ReceiveServerContent( Packet p )
    {
        return ReceiveInvalidPacket( PacketContentType.Server_Content );
    }

    /// <summary>
    /// Handle the given packet, i.e. pass it to the right<br/>
    /// parser receive command.
    /// </summary>
    /// <param name="p">The packet to handle.</param>
    /// <returns><see langword="true"/> if we should immediately handle further packets, <see langword="false"/> otherwise.</returns>
    protected bool HandlePacket( Packet p )
    {
        PacketContentType type = (PacketContentType)p.RecvByte();

        switch ( HasClientQuit() ? PacketContentType.End : type )
        {
            case PacketContentType.Client_InfoList: return ReceiveClientInfoList( p );
            case PacketContentType.Client_InfoID: return ReceiveClientInfoID( p );
            case PacketContentType.Client_InfoExtID: return ReceiveClientInfoExtID( p );
            case PacketContentType.Client_InfoExtID_MD5: return ReceiveClientInfoExtIDMD5( p );
            case PacketContentType.Server_Info: return ReceiveServerInfo( p );
            case PacketContentType.Client_Content: return ReceiveClientContent( p );
            case PacketContentType.Server_Content: return ReceiveServerContent( p );

            default:
                if ( HasClientQuit() )
                {
                    Console.WriteLine( $"[TCP/Content] Received invalid packet type {type}" );
                }
                else
                {
                    Console.WriteLine( "[TCP/Content] Received illegal packet" );
                }
                return false;
        }
    }

    /// <summary>
    /// Receive a packet at TCP level.
    /// </summary>
    /// <returns>Whether at least one packet was received.</returns>
    public bool ReceivePackets()
    {
        // We read only a few of the packets, this as receiving packets can be expensive
        // due to the re-resolving of the parent/child relations and checking the toggle
        // state of all bits, we cannot do this all in one go, as we want to show the
        // user what we already received, otherwise, it can take very long before any
        // progress is shown to the end user that something has been received
        // It is also the case that we request extra content from the content server in
        // case there is an unknown (in the content list) piece of content, these will
        // come in after the main lists have been requested, as a result, we won't be
        // getting everything reliably in one batch, thus, we need to make subsequent
        // updates in that case as well
        //
        // As a result, we simply handle an arbitrary number of packets in one cycle,
        // and let the rest be handled in subsequent cycles, these are ran, almost,
        // immediately after this cycle so in speed it does not matter much, except
        // that the user interface will appear better responding
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

public static partial class NetworkCore
{
    /// <summary>
    /// Helper to get the subdirectory a <see cref="ContentInfo"/> is located in.
    /// </summary>
    /// <param name="type">The type of content.</param>
    /// <returns>The subdirectory the content is located in.</returns>
    public static Subdirectory GetContentInfoSubDir( ContentType type )
    {
        switch ( type )
        {
            default:
                return NO_DIRECTORY;
            case ContentType.AI: return AI_DIR;
            case ContentType.AILibrary: return AI_LIBRARY_DIR;
            case ContentType.Game: return GAME_DIR;
            case ContentType.GameLibrary: return GAME_LIBRARY_DIR;
            case ContentType.NewGRF: return NEWGRF_DIR;

            case ContentType.BaseGraphics:
            case ContentType.BaseSounds:
            case ContentType.BaseMusic:
                return BASESET_DIR;

            case ContentType.Scenario: return SCENARIO_DIR;
            case ContentType.Heightmap: return HEIGHTMAP_DIR;
        }
    }
}