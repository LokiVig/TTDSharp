using System;
using System.Net.Sockets;

namespace OpenTTD.Network.Core;

/// <summary>
/// Abstraction of a network error where all implementation details of the<br/>
/// error codes are encapsulated in this class and the abstraction layer.
/// </summary>
public class NetworkError
{
    private int error; // The underlying error number from errno or WSAGetLastError
    private string message; // The string representation of the error (set on first call to AsString)

    /// <summary>
    /// Construct the network error with the given error code.
    /// </summary>
    /// <param name="error">The error code.</param>
    public NetworkError( int error )
    {
        this.error = error;
    }

    public bool HasError()
    {

    }

    /// <summary>
    /// Check whether this error describes that the operation would block.
    /// </summary>
    /// <returns><see langword="true"/> if the operation would block.</returns>
    public bool WouldBlock()
    {
        throw new NotImplementedException( "This function uses hard-coded variables from both Win32 and POSIX that, afaik, C# doesn't have as much access to. Whoopsies!" );
    }

    /// <summary>
    /// Check whether this error describes a connection reset.
    /// </summary>
    /// <returns><see langword="true"/> if the connection is reset.</returns>
    public bool IsConnectionReset()
    {
        throw new NotImplementedException( "This function uses hard-coded variables from both Win32 and POSIX that, afaik, C# doesn't have as much access to. Whoopsies!" );
    }

    /// <summary>
    /// Check whether this error describes a connect is in progress.
    /// </summary>
    /// <returns><see langword="true"/> if the connect is already in progress.</returns>
    public bool IsConnectInProgress()
    {
        throw new NotImplementedException( "This function uses hard-coded variables from both Win32 and POSIX that, afaik, C# doesn't have as much access to. Whoopsies!" );
    }

    /// <summary>
    /// Get the string representation of the error message.
    /// </summary>
    /// <returns>The string representation that will get overwritten by next calls.</returns>
    public string AsString()
    {
        if ( string.IsNullOrEmpty( message ) )
        {

        }
    }

    public static NetworkError GetLast()
    {

    }
}

public static partial class NetworkCore
{
    /// <summary>
    /// Emscripten doesn't get 'addrlen' for accept(), getsockname(), getpeername()<br/>
    /// and recvfrom(), which confuses other functions and causes them to crash.<br/>
    /// This function needs to be called after these four functions to make sure<br/>
    /// 'addrlen' is patched up.<br/>
    /// <br/>
    /// https://github.com/emscripten-core/emscripten/issues/12996
    /// </summary>
    /// <param name="address">The address returned by those four functions.</param>
    /// <returns>The correct value for addrlen.</returns>
    public static int FixAddrLenForEmscriptem( StockAddrStorage address )
    {
        switch ( address.ssFamily )
        {
            case AddressFamily.InterNetworkV6:
                throw new NotImplementedException( "sizeof(sockaddr_in6)" );

            case AddressFamily.InterNetwork:
                throw new NotImplementedException( "sizeof(sockaddr_in)" );

            default:
                throw new NotImplementedException( "NOT_REACHED()" );
        }
    }

    public static bool SetNonBlocking( Socket d )
    {

    }

    public static bool SetNoDelay( Socket d )
    {

    }

    public static bool SetReusePort( Socket d )
    {

    }

    public static NetworkError GetSocketError( Socket d )
    {

    }
}