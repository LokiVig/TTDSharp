using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using OpenTTD.OS.Windows;

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

    /// <summary>
    /// Construct the network error with the given error code and message.
    /// </summary>
    /// <param name="error">The error code.</param>
    /// <param name="message">The error message. Leave empty to determine this automatically based on the error number.</param>
    public NetworkError( int error, string message )
    {
        this.error = error;
        this.message = message;
    }

    /// <summary>
    /// Check whether an error was actually set.
    /// </summary>
    /// <returns><see langword="true"/> if an error was set.</returns>
    public bool HasError()
    {
        return error != 0;
    }

    /// <summary>
    /// Check whether this error describes that the operation would block.
    /// </summary>
    /// <returns><see langword="true"/> if the operation would block.</returns>
    public bool WouldBlock()
    {
        return error == WindowsSocketsErrorCodes.WSAEWOULDBLOCK;
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
            message = $"Unknown error {error}";
        }

        return message;
    }

    /// <summary>
    /// Get the last network error.
    /// </summary>
    /// <returns>The network error.</returns>
    public static NetworkError GetLast()
    {
        return new NetworkError(Marshal.GetLastWin32Error());
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

    /// <summary>
    /// Try to set the socket into non-blocking mode.
    /// </summary>
    /// <param name="d">The socket to set the non-blocking mode for.</param>
    /// <returns><see langword="true"/> if setting the non-blocking mode succeeded, otherwise <see langword="false"/>.</returns>
    public static bool SetNonBlocking( Socket d )
    {
        return d.Blocking = false;
    }

    /// <summary>
    /// Try to set the socket to not delay sending.
    /// </summary>
    /// <param name="d">The socket to disable the delaying for.</param>
    /// <returns><see langword="true"/> if disabling the delaying succeeded, otherwise <see langword="false"/>.</returns>
    public static bool SetNoDelay( Socket d )
    {
        return d.NoDelay = true;
    }

    /// <summary>
    /// Try to set the socket to reuse ports.
    /// </summary>
    /// <param name="d">The socket to reuse ports on.</param>
    /// <returns><see langword="true"/> if enabling the reusing succeeded, otherwise <see langword="false"/>.</returns>
    public static bool SetReusePort( Socket d )
    {
        d.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true );

        return d.GetSocketOption( SocketOptionLevel.Socket, SocketOptionName.ReuseAddress ) != null;
    }

    /// <summary>
    /// Get the error from a socket, if any.
    /// </summary>
    /// <param name="d">The socket to get the error from.</param>
    /// <returns>A <see cref="NetworkError"/> from the socket.</returns>
    public static NetworkError GetSocketError( Socket d )
    {
        if ( d.GetSocketOption( SocketOptionLevel.Socket, SocketOptionName.Error ) is int error )
        {
            return new NetworkError( error );
        }
        else
        {
            return new NetworkError( -1, "Couldn't get error for socket." );
        }
    }
}