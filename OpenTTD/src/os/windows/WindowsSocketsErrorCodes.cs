namespace OpenTTD.OS.Windows;

/// <summary>
/// Most Windows Sockets 2 functions do not return the specific cause of an error when the function returns.<br/>
/// For information, see the <see href="https://learn.microsoft.com/en-us/windows/win32/winsock/handling-winsock-errors">Handling Winsock Errors</see> topic.<br/>
/// <br/>
/// The <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-wsagetlasterror">WSAGetLastError</see> function returns the last error that occurred for the calling thread.<br/>
/// When a particular Windows Sockets function indicates an error has occurred, this function should be called immediately to retrieve the extended error code for the failing function call.<br/>
/// These error codes and a short text description associated with an error code are defined in the <i>Winerror.h</i> header file.<br/>
/// The <see href="https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-formatmessage">FormatMessage</see> function can be used to obtain the message string for the returned error.<br/>
/// <br/>
/// For information on how to handle error codes when porting socket applications to Winsock, see <see href="https://learn.microsoft.com/en-us/windows/win32/winsock/error-codes-errno-h-errno-and-wsagetlasterror-2">Error codes - errno, h_errno and WSAGetLastError</see>.<br/>
/// </summary>
public static class WindowsSocketsErrorCodes
{
    /// <summary>
    /// Specified event object handle is invalid.<br/>
    /// An application attempts to use an event object, but the specified handle is not valid.
    /// </summary>
    public const int WSA_INVALID_HANDLE = 6;

    /// <summary>
    /// Insufficient memory available.<br/>
    /// An application used a Windows Sockets function that directly maps to a Windows function.<br/>
    /// The Windows function is indicating a lack of required memory resources.
    /// </summary>
    public const int WSA_NOT_ENOUGH_MEMORY = 8;

    /// <summary>
    /// One or more parameters is invalid.<br/>
    /// An application used a Windows Sockets function that directly maps to a Windows function.<br/>
    /// The Windows function is indicating a problem with one or more parameters.
    /// </summary>
    public const int WSA_INVALID_PARAMETER = 87;

    /// <summary>
    /// Overlapped operation aborted.<br/>
    /// An overlapped operation was canceled due to the closure of the socket, or the execution of the SIO_FLUSH command in <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-wsaioctl">WSAIoctl</see>.
    /// </summary>
    public const int WSA_OPERATION_ABORTED = 995;

    /// <summary>
    /// Overlapped I/O event object not in signaled state.<br/>
    /// The application has tried to determine the status of an overlapped operation which is not yet completed.<br/>
    /// Applications that use <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-wsagetoverlappedresult">WSAGetOverlappedResult</see> (with the fWait flag set to <see langword="false"/>) in a polling mode to determine when an overlapped operation has completed, get this error code until the operation is complete.
    /// </summary>
    public const int WSA_IO_INCOMPLETE = 996;

    /// <summary>
    /// Overlapped operations will complete later.<br/>
    /// The application has initiated an overlapped operation that cannot be completed immediately.<br/>
    /// A completion indication will be given later when the operation has been completed.
    /// </summary>
    public const int WSA_IO_PENDING = 997;

    /// <summary>
    /// Interrupted function call.<br/>
    /// A blocking operation was interrupted by a call to <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock2/nf-winsock2-wsacancelblockingcall">WSACancelBlockingCall</see>.
    /// </summary>
    public const int WSAEINTR = 10004;

    /// <summary>
    /// File handle is not valid.<br/>
    /// The file handle supplied is not valid.
    /// </summary>
    public const int WSAEBADF = 10009;

    /// <summary>
    /// Permission denied.<br/>
    /// An attempt was made to access a socket in a way forbidden by its access permissions.<br/>
    /// An example is using a broadcast address for <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-sendto">sendto</see> without broadcast permission being set using <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-setsockopt">setsockopt</see> (SO_BROADCAST).<br/>
    /// Another possible reason for the <see cref="WSAEACCES"/> error is that when the <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-bind">bind</see> function is called (on Windows NT 4.0 with SP4 and later), another application, service or kernel mode driver is bound to the same address with exclusive access.<br/>
    /// Such exclusive access is a new feature of Windows NT 4.0 with SP4 and later, and is implemented by using the <see href="https://learn.microsoft.com/en-us/windows/desktop/winsock/so-exclusiveaddruse">SO_EXCLUSIVEADDRUSE</see> option.
    /// </summary>
    public const int WSAEACCES = 10013;

    /// <summary>
    /// Bad address.<br/>
    /// The system detected an invalid pointer address in attempting to use a pointer argument of a call.<br/>
    /// This error occurs if an application passes an invalid pointer value, or if the length of the buffer is too small.<br/>
    /// For instance, if the length of an argument, which is a <see href="https://learn.microsoft.com/en-us/windows/desktop/winsock/sockaddr-2">sockaddr</see> structure, is smaller than the size of sockaddr.
    /// </summary>
    public const int WSAEFAULT = 10014;

    /// <summary>
    /// Invalid argument.<br/>
    /// Some invalid argument was supplied (for example, specifying an invalid level to the <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-setsockopt">setsockopt</see> function).<br/>
    /// In some instances, it also refers to the current state of the socket---for instance, calling <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-accept">accept</see> on a socket that is not listening.
    /// </summary>
    public const int WSAEINVAL = 10022;

    /// <summary>
    /// Too many open files.<br/>
    /// Too many open sockets. Each implementation may have a maximum number of socket handles available, either globally, per process, or per thread.
    /// </summary>
    public const int WSAEMFILE = 10024;

    /// <summary>
    /// Resource file is temporarily unavailable.<br/>
    /// This error is returned from operations on nonblocking sockets that cannot be completed immediately, for example <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-recv">recv</see> when no data is queued to be read from the socket.<br/>
    /// It is a nonfatal error, and the operation should be retried later.<br/>
    /// It is normal for <see cref="WSAEWOULDBLOCK"/> to be reported as the result from calling <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-connect">connect</see> on a nonblocking SOCK_STREAM socket, since some time must elapse for the connection to be established.
    /// </summary>
    public const int WSAEWOULDBLOCK = 10035;

    /// <summary>
    /// Operation now in progress.<br/>
    /// A blocking operation is currently executing.<br/>
    /// Windows Sockets only allows a single blocking operation---per- task or thread---to be outstanding, and if any other function call is made (whether or not it references that or any other socket) the function fails with the <see cref="WSAEINPROGRESS"/> error.
    /// </summary>
    public const int WSAEINPROGRESS = 10036;

    /// <summary>
    /// Operation already in progress.<br/>
    /// An operation was attempted on a nonblocking socket with an operation already in progress---that is, calling <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-connect">connect</see> a second time on a nonblocking socket that is already connecting, or canceling an asynchronous request (<b>WSAAsyncGetXbyY</b>) that has already been canceled or completed.
    /// </summary>
    public const int WSAEALREADY = 10037;

    /// <summary>
    /// Socket operation on nonsocket.<br/>
    /// An operation was attempted on something that is not a socket.<br/>
    /// Either the socket handle parameter did not reference a valid socket, or for <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-select">select</see>, a member of an <b>fd_set</b> was not valid.
    /// </summary>
    public const int WSAENOTSOCK = 10038;

    /// <summary>
    /// Destination address required.<br/>
    /// A required address was omitted from an operation on a socket.<br/>
    /// For example, this error is returned if <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-sendto">sendto</see> is called with the remote address of ADDR_ANY.
    /// </summary>
    public const int WSAEDESTADDREQ = 10039;

    /// <summary>
    /// Message too long.<br/>
    /// A message sent on a datagram socket was larger than the internal message buffer or some other network limit, or the buffer used to receive a datagram was smaller than the datagram itself.
    /// </summary>
    public const int WSAEMSGSIZE = 10040;

    /// <summary>
    /// Protocol wrong type for socket.<br/>
    /// A protocol was specified in the <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-socket">socket</see> function call that does not support the semantics of the socket type requested.<br/>
    /// For example, the ARPA Internet UDP protocol cannot be specified with a socket type of SOCK_STREAM.
    /// </summary>
    public const int WSAEPROTOTYPE = 10041;

    /// <summary>
    /// Bad protocol option.<br/>
    /// An unknown, invalid or unsupported option or level was specified in a <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-getsockopt">getsockopt</see> or <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-setsockopt">setsockopt</see> call.
    /// </summary>
    public const int WSAENOPROTOOPT = 10042;

    /// <summary>
    /// Protocol not supported.<br/>
    /// The requested protocol has not been configured into the system, or no implementation for it exists.<br/>
    /// For example, a <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-socket">socket</see> call requests a SOCK_DGRAM socket, but specifies a stream protocol.
    /// </summary>
    public const int WSAEPROTONOSUPPORT = 10043;

    /// <summary>
    /// Socket type not supported.<br/>
    /// The support for the specified socket type does not exist in this address family.<br/>
    /// For example, the optional type SOCK_RAW might be selected in a <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-socket">socket</see> call, and the implementation does not support SOCK_RAW sockets at all.
    /// </summary>
    public const int WSAESOCKTNOSUPPORT = 10044;

    /// <summary>
    /// Operation not supported.<br/>
    /// The attempted operation is not supported for the type of object referenced.<br/>
    /// Usually this occurs when a socket descriptor to a socket that cannot support this operation is trying to accept a connection on a datagram socket.
    /// </summary>
    public const int WSAEOPNOTSUPP = 10045;

    /// <summary>
    /// Protocol family not supported.<br/>
    /// The protocol family has not been configured into the system or no implementation for it exists.<br/>
    /// This message has a slightly different meaning from <see cref="WSAEAFNOSUPPORT"/>.<br/>
    /// However, it is interchangeable in most cases, and all Windows Sockets functions that return one of these messages also specify <see cref="WSAEAFNOSUPPORT"/>.
    /// </summary>
    public const int WSAEPFNOSUPPORT = 10046;

    /// <summary>
    /// Address family not supported by protocol family.<br/>
    /// An address incompatible with the requested protocol was used.<br/>
    /// All sockets are created with an associated address family (that is, AF_INET for Internet Protocols) and a generic protocol type (that is, SOCK_STREAM).<br/>
    /// This error is returned if an incorrect protocol is explicitly requested in the <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-socket">socket</see> call, or if an address of the wrong family is used for a socket, for example, in <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-sendto">sendto</see>.
    /// </summary>
    public const int WSAEAFNOSUPPORT = 10047;

    /// <summary>
    /// Address already in use.<br/>
    /// Typically, only one usage of each socket address (protocol/IP address/port) is permitted.<br/>
    /// This error occurs if an application attempts to <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-bind">bind</see> a socket to an IP address/port that has already been used for an existing socket, or a socket that was not closed properly, or one that that is still in the process of closing.<br/>
    /// For server applications that need to <b>bind</b> multiple sockets to the same port number, consider using <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-setsockopt">setsockopt</see> (SO_REUSEADDR).<br/>
    /// Client applications usually need not call <b>bind</b> at all---<see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-connect">connect</see> chooses an unused port automatically.<br/>
    /// When <b>bind</b> is called with a wildcard address (involving ADDR_ANY), a <see cref="WSAEADDRINUSE"/> error could be delayed until the specific address is committed.<br/>
    /// This could happen with a call to another function later, including <b>connect</b>, <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-listen">listen</see>, <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-wsaconnect">WSAConnect</see>, or <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-wsajoinleaf">WSAJoinLeaf</see>.
    /// </summary>
    public const int WSAEADDRINUSE = 10048;
}