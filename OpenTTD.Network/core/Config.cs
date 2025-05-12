using System;

namespace OpenTTD.Network.Core;

public static class Config
{
    public const ushort NETWORK_COORDINATOR_SERVER_PORT = 3976; // The default port of the Game Coordinator server (TCP)
    public const ushort NETWORK_STUN_SERVER_PORT = 3976; // The default port of the STUN server (TCP)
    public const ushort NETWORK_TURN_SERVER_PORT = 3976; // The default port of the TURN server (TCP)
    public const ushort NETWORK_CONTENT_SERVER_PORT = 3976; // The default port of the content server (TCP)
    public const ushort NETWORK_DEFAULT_PORT = 3976; // The default port of the game server (TCP & UDP)
    public const ushort NETWORK_ADMIN_PORT = 3976; // The default port for admin network

    public const int UDP_MTU = 1460; // Number of bytes we can pack in a single UDP packet

    public const string NETWORK_SURVEY_DETAILS_LINK = "https://survey.openttd.org/participate"; // Link with some more details & privacy statement of the survey

    //
    // Technically a TCP packet could become 64KiB, however the high bit is kept so it becomes possible in the future
    // to go to (significantly) larger packets if needed. This would entail a strategy such as employed for UTF-8.
    //
    // Packets up to 32 KiB have the high bit not set:
    // 00000000 00000000 0bbbbbbb aaaaaaaa -> aaaaaaaa 0bbbbbbb
    // SendUShort(size, 0, 15)
    //
    // Packets up to 1 GiB, first ushort has high bit set so it knows to read a
    // next ushort for the remaining bits of the size.
    // 00dddddd cccccccc bbbbbbbb aaaaaaaa -> cccccccc 10dddddd aaaaaaaa bbbbbbbb
    // SendUShort(BitMath.GB(size, 16, 14) | 0b10 << 14)
    // SendUShort(BitMath.GB(size,  0, 16))
    //

    public const int TCP_MTU = 32767; // Number of bytes we can pack in a single TCP packet
    public const int COMPAT_MTU = 1460; // Number of bytes we can pack in a single packet for backwards compatibility

    public const byte NETWORK_GAME_ADMIN_VERSION = 3; // What version of the admin network do we use?
    public const byte NETWORK_GAME_INFO_VERSION = 7; // What version of game-info do we use?
    public const byte NETWORK_COORDINATOR_VERSION = 6; // What version of game-coordinator-protocol do we use?
    public const byte NETWORK_SURVEY_VERSION = 2; // What version of the survey do we use?

    public const uint NETWORK_NAME_LENGTH = 80; // The maximum length of the server name and map name, in bytes including '\0'
    public const uint NETWORK_HOSTNAME_LENGTH = 80; // The maximum length of the host name, in bytes including '\0'
    public const uint NETWORK_HOSTNAME_PORT_LENGTH = 80 + 6; // The maximum length of the host name + port, in bytes including '\0', the extra six is ":" + port number (with a max of 65536)
    public const uint NETWORK_REVISION_LENGTH = 33; // The maximum length of the revision, in bytes including '\0'
    public const uint NETWORK_PASSWORD_LENGTH = 33; // The maximum length of the password, in bytes including '\0'
    public const uint NETWORK_CLIENT_NAME_LENGTH = 25; // The maximum length of a client's name, in bytes including '\0'
    public const uint NETWORK_RCONCOMMAND_LENGTH = 500; // The maximum length of a rconsole command, in bytes including '\0'
    public const uint NETWORK_GAMESCRIPT_JSON_LENGTH = 9000; // The maximum length of a receiving gamescript json string, in bytes including '\0'
    public const uint NETWORK_CHAT_LENGTH = 900; // The maximum length of a chat message, in bytes including '\0'
    public const uint NETWORK_CONTENT_FILENAME_LENGTH = 48; // The maximum length of a content's filename, in bytes including '\0'
    public const uint NETWORK_CONTENT_NAME_LENGTH = 32; // The maximum length of a content's name, in bytes including '\0'
    public const uint NETWORK_CONTENT_VERSION_LENGTH = 16; // The maximum length of a content's version, in bytes including '\0'
    public const uint NETWORK_CONTENT_URL_LENGTH = 96; // The maximum length of a content's url, in bytes including '\0'
    public const uint NETWORK_CONTENT_DESC_LENGTH = 512; // The maximum length of a content's description, in bytes including '\0'
    public const uint NETWORK_CONTENT_TAG_LENGTH = 32; // The maximum length of a content's tag, in bytes including '\0'
    public const uint NETWORK_ERROR_DETAIL_LENGTH = 100; // The maximum length of the error detail, in bytes including '\0'
    public const uint NETWORK_INVITE_CODE_LENGTH = 64; // The maximum length of the invite code, in bytes including '\0'
    public const uint NETWORK_INVITE_CODE_SECRET_LENGTH = 80; // The maximum length of the invite code secret, in bytes including '\0'
    public const uint NETWORK_TOKEN_LENGTH = 64; // The maximum length of a token, in bytes including '\0'

    public const uint NETWORK_GRF_NAME_LENGTH = 80; // The maximum length of the name of a GRF

    //
    // Maximum number of GRFs that can be sent.
    //
    // This limit exists to avoid that the SERVER_INFO packet exceedign the
    // maximum MTU. At the time of writing this limit is 32767 (TCP_MTU).
    //
    // In the SERVER_INFO packet is the NetworkGameInfo struct, which is
    // 142 bytes + 100 per NewGRF (under the assumption strings are used to
    // their max). This brings us to roughly 326 possible NewGRFs. Round it
    // down so people don't freak out because they see a weird value, and you
    // get the limit: 255.
    //
    // PS: In case you ever want to raise this number, please be mindful that
    // "amount of NewGRFs" in NetworkGameInfo is currently a byte.
    //
    public const uint NETWORK_MAX_GRF_COUNT = 255;

    //
    // The maximum length of the hexadecimal encoded secret keys, in bytes including '\0'.
    // This is related to X25519_KEY_SIZE in the network crypto internals.
    //
    public const uint NETWORK_SECRET_KEY_LENGTH = 32 * 2 + 1;

    //
    // The maximum length of the hexadecimal encoded public keys, in bytes including '\0'.
    // This is related to X25519_KEY_SIZE in the network crypto internals.
    //
    public const uint NETWORK_PUBLIC_KEY_LENGTH = 32 * 2 + 1;

    /// <summary>
    /// Get the connection string for the game coordinator from the environment variable <see cref="OTTD_COORDINATOR_CS"/>,<br/>
    /// or when it has not been set a hard coded default DNS hostname of the production server.
    /// </summary>
    /// <returns>The game coordinator's connection string.</returns>
    public static string NetworkCoordinatorConnectionString()
    {
        return GetEnv( "OTTD_COORDINATOR_CS", "coordinator.openttd.org" );
    }

    /// <summary>
    /// Get the connection string for the STUN server from the environment variable <see cref="OTTD_STUN_CS"/>,<br/>
    /// or when it has not been set a hard coded default DNS hostname of the production server.
    /// </summary>
    /// <returns>The STUN server's connection string.</returns>
    public static string NetworkStunConnectionString()
    {
        return GetEnv( "OTTD_STUN_CS", "stun.openttd.org" );
    }

    /// <summary>
    /// Get the connection string for the content server from the environment variable <see cref="OTTD_CONTENT_SERVER_CS"/>,<br/>
    /// or when it has not been set a hard coded default DNS hostname of the production server.
    /// </summary>
    /// <returns>The content server's connection string.</returns>
    public static string NetworkContentServerConnectionString()
    {
        return GetEnv( "OTTD_CONTENT_SERVER_CS", "content.openttd.org" );
    }

    /// <summary>
    /// Get the URI string for the content mirror from the environment variable <see cref="OTTD_CONTENT_MIRROR_URI"/>,<br/>
    /// or when it has not been set a hard coded URI of the production server.
    /// </summary>
    /// <returns>The content mirror's URI string.</returns>
    public static string NetworkContentMirrorUriString()
    {
        return GetEnv( "OTTD_CONTENT_MIRROR_URI", "https://binaries.openttd.org/bananas" );
    }

    /// <summary>
    /// Get the URI string for the survey from the environment variable <see cref="OTTD_SURVEY_URI"/>,<br/>
    /// or when it has not been set a hard coded URI of the production server.
    /// </summary>
    /// <returns>The survey's URI string.</returns>
    public static string NetworkSurveyUriString()
    {
        return GetEnv( "OTTD_SURVEY_URI", "https://survey-participate.openttd.org" );
    }

    /// <summary>
    /// Get the environment variable using <see cref="Environment.GetEnvironmentVariable(string)"/> when it is an empty <see langword="string"/> (or <see langword="null"/>), return a fallback value instead.
    /// </summary>
    /// <param name="variable">The environment variable to read from.</param>
    /// <param name="fallback">The fallback in case the environment variable is not set.</param>
    /// <returns>The environment value, or when that doesn't exist, the given fallback value.</returns>
    public static string GetEnv( string variable, string fallback )
    {
        string value = Environment.GetEnvironmentVariable( variable );
        return string.IsNullOrEmpty( value ) ? fallback : value;
    }
}