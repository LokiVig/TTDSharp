using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace OpenTTD.Network.Core;

/// <summary>
/// Wrapper for (un)resolved network addresses; there's no reason to transform<br/>
/// a numeric IP to a string and then back again to pass it to functions.<br/>
/// It furthermore allows easier delaying of the hostname lookup.
/// </summary>
public class NetworkAddress
{
    private string hostname; // The hostname
    private int addressLength; // The length of the resolved address
    private StockAddrStorage address; // The resolved address
    private bool resolved; // Whether the address has been (tried to be) resolved

    /// <summary>
    /// Helper function to resolve something to a socket.
    /// </summary>
    /// <param name="runp">Information about the socket to try not.</param>
    /// <returns>The opened socket or <see langword="null"/>.</returns>
    public delegate Socket LoopProc( AddrInfo runp );

    /// <summary>
    /// Create a network address based on a resolved IP and port.
    /// </summary>
    /// <param name="address">The IP address with port.</param>
    /// <param name="addressLength">The length of the address.</param>
    public NetworkAddress( SockAddrStorage address, int addressLength )
    {
        this.addressLength = addressLength;
        this.address = address;
        resolved = addressLength != 0;
    }

    /// <summary>
    /// Create a network address based on a resolved IP and port.
    /// </summary>
    /// <param name="address">The IP address with port.</param>
    /// <param name="addressLength">The length of the address.</param>
    public NetworkAddress( SockAddr address, int addressLength )
    {
        this.addressLength = addressLength;
        resolved = addressLength != 0;

        this.address = address;
    }

    /// <summary>
    /// Create a network address based on a resolved IP and port.
    /// </summary>
    /// <param name="hostname">The unresolved hostname.</param>
    /// <param name="port">The port.</param>
    /// <param name="family">The address family.</param>
    public NetworkAddress( string hostname = "", ushort port = 0, AddressFamily family = AddressFamily.Unspecified )
    {
        addressLength = 0;
        resolved = false;

        if ( !string.IsNullOrEmpty( hostname ) && hostname.StartsWith( "[" ) && hostname.EndsWith( "]" ) )
        {
            hostname.Remove( 0, 1 );
            hostname.Remove( hostname.Length - 1, 1 );
        }

        this.hostname = hostname;
        address.ssFamily = family;
        SetPort( port );
    }

    /// <summary>
    /// Get the hostname; in case it wasn't given the<br/>
    /// IPv4 dotted representation is given.
    /// </summary>
    /// <returns>The hostname.</returns>
    public string GetHostname()
    {
        if ( string.IsNullOrEmpty( hostname ) && address.ssFamily != AddressFamily.Unspecified )
        {
            Debug.Assert( addressLength != 0 );
            // char buffer[NETWORK_HOSTNAME_LENGTH];
            // getnameinfo((struct sockaddr*)&this->address, this->address_length, buffer, sizeof(buffer), nullptr, 0, NI_NUMERICHOST);
            // this->hostname = buffer;
            throw new NotImplementedException( "What's the C# version of `getnameinfo(sockaddr*, socklen_t, char[], size_t, char*, char, int)`???" );
        }

        return hostname;
    }

    /// <summary>
    /// Get the address as string, e.g. 127.0.0.1:12345.
    /// </summary>
    /// <param name="withFamily">Whether to add the family (e.g. IPvX).</param>
    /// <returns>The address.</returns>
    public string GetAddressAsString( bool withFamily = true )
    {
        return string.Format( GetAddressFormatString( GetAddress().ssFamily, withFamily ), GetHostname(), GetPort() );
    }

    /// <summary>
    /// Get the address in its internal representation.
    /// </summary>
    /// <returns>The address.</returns>
    public SockAddrStorage GetAddress()
    {
        if ( IsResolved() )
        {
            // Here we try to resolve a network address, we use SOCK_STREAM as
            // socket type because some stupid OSes, like Solaris, cannot be
            // bothered to implement the specifications and allow '0' as value
            // that means "don't care whether it is SOCK_STREAM or SOCK_DGRAM"
            Resolve( address.ssFamily, SocketType.Stream, AI_ADDRCONFIG, null, ResolveLoopProc );
            resolved = true;
        }

        return address;
    }

    /// <summary>
    /// Get the (valid) length of the address.
    /// </summary>
    /// <returns>The length.</returns>
    public int GetAddressLength()
    {
        // Resolve it if we didn't do it already
        if ( !IsResolved() )
        {
            GetAddress();
        }

        return addressLength;
    }

    /// <summary>
    /// Get the port.
    /// </summary>
    /// <returns>The port.</returns>
    public ushort GetPort()
    {
        switch ( address.ssFamily )
        {
            case AddressFamily.Unspecified:
            case AddressFamily.InterNetwork:
                throw new NotImplementedException( "What's `ntohs`?" );

            case AddressFamily.InterNetworkV6:
                throw new NotImplementedException( "What's `ntohs`?" );

            default:
                throw new NotImplementedException( "NOT_REACHED()" );
        }
    }

    /// <summary>
    /// Set the port.
    /// </summary>
    /// <param name="port">Set the port number.</param>
    public void SetPort( ushort port )
    {
        switch ( address.ssFamily )
        {
            case AddressFamily.Unspecified:
            case AddressFamily.InterNetwork:
                throw new NotImplementedException( "((struct sockaddr_in*)&this->address)->sin_port = htons(port);" );

            case AddressFamily.InterNetworkV6:
                throw new NotImplementedException( "((struct sockaddr_in*)&this->address)->sin6_port = htons(port);" );

            default:
                throw new NotImplementedException( "NOT_REACHED()" );
        }
    }

    /// <summary>
    /// Resolve this address into a socket.
    /// </summary>
    /// <param name="family">The type of 'protocol' (IPv4, IPv6).</param>
    /// <param name="socktype">The type of socket (TCP, UDP, etc.).</param>
    /// <param name="flags">The flags to send to getaddrinfo.</param>
    /// <param name="sockets">The list of sockets to add the sockets to.</param>
    /// <param name="func">The inner working while looping over the address info.</param>
    /// <returns>The resolved socket or <see langword="null"/>.</returns>
    public Socket Resolve( AddressFamily family, SocketType socktype, int flags, SocketList sockets, LoopProc func )
    {
        AddrInfo ai;
        AddrInfo hints = new AddrInfo();
        hints.aiFamily = family;
        hints.aiFlags = flags;
        hints.aiSockType = socktype;

        // The port needs to be a string, six is enough to contain all characters + '\0'
        string portName = GetPort().ToString();

        bool resetHostname = false;

        // Setting both hostname to null and port to 0 is not allowed
        // As port 0 means bind to any port, the other must mean that
        // we want to bind to 'all' IPs
        if ( string.IsNullOrEmpty( hostname ) && addressLength == 0 && GetPort() == 0 )
        {
            resetHostname = true;
            AddressFamily fam = address.ssFamily;

            if ( fam == AddressFamily.Unspecified )
            {
                fam = family;
            }

            hostname = fam == AddressFamily.InterNetwork ? "0.0.0.0" : "::";
        }

        throw new NotImplementedException( "Bullshit." );
    }

    /// <summary>
    /// Check whether the IP address has been resolved already.
    /// </summary>
    /// <returns><see langword="true"/> if the port has been resolved.</returns>
    public bool IsResolved()
    {
        return resolved;
    }

    /// <summary>
    /// Checks if this address is of the given family.
    /// </summary>
    /// <param name="family">The family to check against.</param>
    /// <returns><see langword="true"/> if it is of the given family.</returns>
    public bool IsFamily( int family )
    {
        if ( !IsResolved() )
        {
            Resolve( family, SocketType.Stream, AI_ADDRCONFIG, null, ResolveLoopProc );
        }

        return address.ssFamily == family;
    }

    /// <summary>
    /// Checks whether this IP address is contained by the given netmask.<br/>
    /// NOTE: Netmask without /n assumes all bits need to match.
    /// </summary>
    /// <param name="netmask">The netmask in CIDR notation to test against.</param>
    /// <returns><see langword="true"/> if the IP is within the <paramref name="netmask"/>.</returns>
    public bool IsInNetmask( string netmask )
    {
        // Resolve if we didn't do it already
        if ( !IsResolved() )
        {
            GetAddress();
        }

        int cidr = address.ssFamily == AddressFamily.InterNetwork ? 32 : 128;

        NetworkAddress maskAddress;

        // Check for CIDR separator
        int cidrSeparatorLocation = netmask.IndexOf( '/' );

        if ( cidrSeparatorLocation != 0 )
        {
            int tmpCidr = int.Parse( netmask.Substring( cidrSeparatorLocation + 1 ) );

            // Invalid CIDR, treat as single host
            if ( tmpCidr > 0 && tmpCidr < cidr )
            {
                cidr = tmpCidr;
            }

            // Remove the / so that NetworkAddress works on the IP portion
            maskAddress = new NetworkAddress( netmask.Substring( 0, cidrSeparatorLocation ), 0, address.ssFamily );
        }
        else
        {
            maskAddress = new NetworkAddress( netmask, 0, address.ssFamily );
        }

        if ( maskAddress.GetAddressLength() == 0 )
        {
            return false;
        }

        uint ip;
        uint mask;

        switch ( address.ssFamily )
        {
            case AddressFamily.InterNetwork:
                throw new NotImplementedException( "This is a lot of bogus." );

            case AddressFamily.InterNetworkV6:
                throw new NotImplementedException( "This is a lot of bogus." );

            default:
                throw new NotImplementedException( "NOT_REACHED()" );
        }

        // There's a lot here that needs to be implemented...

        return true;
    }

    /// <summary>
    /// Compare the address of this class with the address of another.
    /// </summary>
    /// <param name="address">The other address.</param>
    /// <returns><c>&lt; 0</c> if address is less, <c>0</c> if equal and <c>&gt; 0</c> if address is more.</returns>
    public int CompareTo( NetworkAddress address )
    {
        int r = GetAddressLength() - address.GetAddressLength();

        if ( r == 0 )
        {
            r = this.address.ssFamily - address.address.ssFamily;
        }

        if ( r == 0 )
        {
            r = addressLength - address.addressLength;
        }

        if ( r == 0 )
        {
            r = GetPort() - address.GetPort();
        }

        return r;
    }

    /// <summary>
    /// Compare the address of <paramref name="lhs"/> with the address of <paramref name="rhs"/>.
    /// </summary>
    /// <param name="lhs">Left hand side <see cref="NetworkAddress"/> variable.</param>
    /// <param name="rhs">Right hand side <see cref="NetworkAddress"/> variable.</param>
    /// <returns><see langword="true"/> if both match.</returns>
    public static bool operator ==( NetworkAddress lhs, NetworkAddress rhs )
    {
        return lhs.CompareTo( rhs ) == 0;
    }

    /// <summary>
    /// Compare the address of <paramref name="lhs"/> with the address of <paramref name="rhs"/>.
    /// </summary>
    /// <param name="lhs">Left hand side <see cref="NetworkAddress"/> variable.</param>
    /// <param name="rhs">Right hand side <see cref="NetworkAddress"/> variable.</param>
    /// <returns><see langword="true"/> if neither match.</returns>
    public static bool operator !=( NetworkAddress lhs, NetworkAddress rhs )
    {
        return lhs.CompareTo( rhs ) != 0;
    }

    /// <summary>
    /// Make the given socket listen.
    /// </summary>
    /// <param name="socktype">The type of socket (TCP, UDP, etc.)</param>
    /// <param name="sockets">The list of sockets to add the sockets to.</param>
    public void Listen( SocketType socktype, SocketList sockets )
    {
        Debug.Assert( sockets != null );

        // Setting both hostname to "" and port to 0 is not allowed
        // As port 0 means bind to any port, the other must mean that
        // we want to bind to 'all' IPs
        if ( addressLength == 0 && address.ssFamily == AddressFamily.Unspecified && string.IsNullOrEmpty(hostname) && GetPort() == 0)
        {
            Resolve( AddressFamily.InterNetwork, socktype, AI_ADDRCONFIG | AI_PASSIVE, sockets, ListenLoopProc );
            Resolve( AddressFamily.InterNetworkV6, socktype, AI_ADDRCONFIG | AI_PASSIVE, sockets, ListenLoopProc );
        }
        else
        {
            Resolve( AddressFamily.Unspecified, socktype, AI_ADDRCONFIG | AI_PASSIVE, sockets, ListenLoopProc );
        }
    }

    /// <summary>
    /// Convert the socket type into a string.<br/>
    /// NOTE: Only works for <see cref="SocketType.Stream"/> and <see cref="SocketType.Dgram"/>.
    /// </summary>
    /// <param name="socktype">The socket type to convert.</param>
    /// <returns>The string representation.</returns>
    public static string SocketTypeAsString( SocketType socktype )
    {
        switch ( socktype )
        {
            case SocketType.Stream:
                return "tcp";

            case SocketType.Dgram:
                return "udp";

            default:
                return "unsupported";
        }
    }

    /// <summary>
    /// Convert the address family into a string.<br/>
    /// NOTE: Only works for <see cref="AddressFamily.InterNetwork"/>, <see cref="AddressFamily.InterNetworkV6"/> and <see cref="AddressFamily.Unspecified"/>.
    /// </summary>
    /// <param name="family">The family to convert.</param>
    /// <returns>The string representation.</returns>
    public static string AddressFamilyAsString( AddressFamily family )
    {
        switch ( family )
        {
            case AddressFamily.Unspecified:
                return "either IPv4 or IPv6";

            case AddressFamily.InterNetwork:
                return "IPv4";

            case AddressFamily.InterNetworkV6:
                return "IPv6";

            default:
                return "unsupported";
        }
    }

    /// <summary>
    /// Get the peer address of a socket as <see cref="NetworkAddress"/>.
    /// </summary>
    /// <param name="sock">The socket to get the peer address of.</param>
    /// <returns>The <see cref="NetworkAddress"/> of the peer address.</returns>
    public static NetworkAddress GetPeerAddress( Socket sock )
    {

    }

    public static NetworkAddress GetSockAddress( Socket sock )
    {

    }

    public static string GetPeerName( Socket sock )
    {

    }

    /// <summary>
    /// Helper to get the formatting string of an address of a given family.
    /// </summary>
    /// <param name="family">The family to get the address format for.</param>
    /// <param name="withFamily">Whether to add the family to the address (e.g. IPv4).</param>
    /// <returns>The format <see langword="string"/> for the address.</returns>
    public static string GetAddressFormatString( AddressFamily family, bool withFamily )
    {
        switch ( family )
        {
            case AddressFamily.InterNetwork:
                return withFamily ? "{0}:{1} (IPv4)" : "{0}:{1}";

            case AddressFamily.InterNetworkV6:
                return withFamily ? "[{0}]:{1} (IPv6)" : "[{0}]:{1}";

            default:
                return withFamily ? "{0}:{1} (IPv?)" : "{0}:{1}";
        }
    }

    /// <summary>
    /// Helper function to resolve without opening a socket.
    /// </summary>
    /// <param name="runp">Information about the socket to try.</param>
    /// <returns>The opened socket or <see langword="null"/>.</returns>
    public static Socket ResolveLoopProc( AddrInfo runp )
    {
        string address = new NetworkAddress( runp.aiAddr, (int)runp.aiAddrLen ).GetAddressLength();

        Socket sock = new Socket( runp.aiFamily, runp.aiSocktype, runp.aiProtocol );

        if ( sock == null )
        {
            string type = SocketTypeAsString( runp.aiSocktype );
            string family = AddressFamilyAsString( runp.aiFamily );
            Console.WriteLine( $"Could not create {type} {family} socket: {NetworkError.GetLast().AsString()}" );
            return null;
        }

        if ( runp.aiSocktype == SocketType.Stream && !SetNoDelay( sock ) )
        {
            Console.WriteLine( $"Setting no-delay mode failed: {NetworkError.GetLast().AsString()}" );
        }

        if ( !SetReusePort( sock ) )
        {
            Console.WriteLine( $"Setting reuse-address mode failed: {NetworkError.GetLast().AsString()}" );
        }

        int on = 1;

        if ( runp.aiFamily == AddressFamily.InterNetworkV6 && sock.SetSocketOption( SocketOptionLevel.IPv6, SocketOptionName.IPOptions, on == 1 ) )
        {
            Console.WriteLine( $"Couldn't disable IPv4 over IPv6: {NetworkError.GetLast().AsString()}" );
        }

        if ( sock.Bind( runp.aiAddr ) != 0 )
        {
            Console.WriteLine( $"Couldn't bind socket on {address}: {NetworkError.GetLast().AsString()}" );
            sock.Close();
            return null;
        }

        if ( runp.aiSocktype == SocketType.Dgram && sock.Listen( 1 ) )
        {
            Console.WriteLine( $"Couldn't listen on socket: {NetworkError.GetLast().AsString()}" );
            sock.Close();
            return null;
        }

        // Connection succeeded
        if ( !SetNonBlocking( sock ) )
        {
            Console.WriteLine( $"Setting non-blocking mode failed: {NetworkError.GetLast().AsString()}" );
        }

        Console.WriteLine( $"Listening on {address}" );
        return sock;
    }
}

/// <summary>
/// Types of server addresses we know.<br/>
/// <br/>
/// Sorting will prefer entries at the top of this list above ones at the bottom.
/// </summary>
public enum ServerAddressType : byte
{
    Direct, // Server-address is based on hostname:port
    InviteCode // Server-address is based on an invite code
}

/// <summary>
/// Address to a game server.<br/>
/// <br/>
/// This generalises addresses which are based on different identifiers.
/// </summary>
public class ServerAddress
{
    public ServerAddressType type;
    public string connectionString;

    /// <summary>
    /// Create a new ServerAddress object.<br/>
    /// <br/>
    /// Please use <see cref="Parse"/> instead of calling this directly.
    /// </summary>
    /// <param name="type">The type of the ServerAddress.</param>
    /// <param name="connectionString">The connectionString that belongs to this ServerAddress type.</param>
    private ServerAddress( ServerAddressType type, string connectionString )
    {
        this.type = type;
        this.connectionString = connectionString;
    }

    public static ServerAddress Parse( string connectionString, ushort defaultPort, CompanyID companyId = null )
    {

    }
}