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
public static class WSErrorCodes
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

    /// <summary>
    /// Cannot assign requested address.<br/>
    /// The requesed address is not valid in its context.<br/>
    /// This normally results from an attempt to <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-bind">bind</see> to an address that is not valid for the local computer.<br/>
    /// This can also result from <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-connect">connect</see>, <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-sendto">sendto</see>, <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-wsaconnect">WSAConnect</see>, <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-wsajoinleaf">WSAJoinLeaf</see>, or <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-wsasendto">WSASendTo</see> when the remote address or port is not valid for a remote computer (for example, address or port 0).
    /// </summary>
    public const int WSAEADDRNOTAVAIL = 10049;

    /// <summary>
    /// Network is down.<br/>
    /// A socket operation encountered a dead network.<br/>
    /// This could indicate a serious failure of the network system (that is, the protocol stack that the Windows Sockets DLL runs over), the network interface, or the local network itself.
    /// </summary>
    public const int WSAENETDOWN = 10050;

    /// <summary>
    /// Network is unreachable.<br/>
    /// A socket operation was attempted to an unreachable network.<br/>
    /// This usually means the local software knows no route to reach the remote host.
    /// </summary>
    public const int WSAENETUNREACH = 10051;

    /// <summary>
    /// Network dropped connection on reset.<br/>
    /// The connection has been broken due to keep-alive activity detecting a failure while the operation was in progress.<br/>
    /// It can also be returned by <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-setsockopt">setsockopt</see> if an attempt is made to set <see href="https://learn.microsoft.com/en-us/windows/desktop/winsock/so-keepalive">SO_KEEPALIVE</see> on a connection that has already failed.
    /// </summary>
    public const int WSAENETRESET = 10052;

    /// <summary>
    /// Software caused connection abort.<br/>
    /// An established connection was aborted by the software in your host computer, possibly due to a data transmission time-out or protocol error.
    /// </summary>
    public const int WSAECONNABORTED = 10053;

    /// <summary>
    /// Connection reset by peer.<br/>
    /// An existing connection was forcibly closed by the remote host.<br/>
    /// This normally results if the peer application on the remote host is suddenly stopped, the host is rebooted, the host or remote network interface is disabled, or the remote host uses a hard close (see <see href="https://learn.microsoft.com/en-us/windows/desktop/winsock/so-keepalive">setsockopt</see> for more information on the SO_LINGER option on the remote socket).<br/>
    /// This error may also result if a connection was broken due to keep-alive activity detecting a failure while one or more operations are in progress.<br/>
    /// Operations that were in progress fail with <see cref="WSAECONNRESET"/>.
    /// </summary>
    public const int WSAECONNRESET = 10054;

    /// <summary>
    /// No buffer space available.<br/>
    /// An operation on a socket could not be performed because the system lacked sufficient buffer space or because a queue was full.
    /// </summary>
    public const int WSAENOBUFS = 10055;

    /// <summary>
    /// Socket is already connected.<br/>
    /// A connect request was made on an already-connected socket.<br/>
    /// Some implementations also return this error if <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-sendto">sendto</see> is called on a connected SOCK_DGRAM socket (for SOCK_STREAM sockets, the <i>to</i> parameter in <b>sendto</b> is ignored) although other implementations treat this as a legal occurrence.
    /// </summary>
    public const int WSAEISCONN = 10056;

    /// <summary>
    /// Socket is not connected.<br/>
    /// A request to send or receive data was disallowed because the socket is not connected and (when sending on a datagram socket using <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-sendto">sendto</see>) no address was supplied.<br/>
    /// Any other type of operation might also return this error---for example, <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-setsockopt">setsockopt</see> setting <see href="https://learn.microsoft.com/en-us/windows/desktop/winsock/so-keepalive">SO_KEEPALIVE</see> if the connection has been reset.
    /// </summary>
    public const int WSAENOTCONN = 10057;

    /// <summary>
    /// Cannot send after socket shutdown.<br/>
    /// A request to send or receive data was disallowed because the socket had already been shut down in that direction with a previous <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-shutdown">shutdown</see> call.<br/>
    /// By calling <b>shutdown</b> a partial close of a socket is requested, which is a signal that sending or receiving, or both have been discontinued.
    /// </summary>
    public const int WSAESHUTDOWN = 10058;

    /// <summary>
    /// Too many references.<br/>
    /// Too many references to some kernel object.
    /// </summary>
    public const int WSAETOOMANYREFS = 10059;

    /// <summary>
    /// Connection timed out.<br/>
    /// A connection attempt failed because the connected party did not properly respond after a period of time, or the established connection failed because the connected host has failed to respond.
    /// </summary>
    public const int WSAETIMEDOUT = 10060;

    /// <summary>
    /// Connection refused.<br/>
    /// No connection could be made because the target computer actively refused it.<br/>
    /// This usually results from trying to connect to a service that is inactive on the foreign host---that is, one with no server application running.
    /// </summary>
    public const int WSAECONNREFUSED = 10061;

    /// <summary>
    /// Cannot translate name.<br/>
    /// Cannot translate a name.
    /// </summary>
    public const int WSAELOOP = 10062;

    /// <summary>
    /// Name too long.<br/>
    /// A name component or a name was too long.
    /// </summary>
    public const int WSAENAMETOOLONG = 10063;

    /// <summary>
    /// Host is down.<br/>
    /// A socket operation failed because the destination host is down.<br/>
    /// A socket operation encountered a dead host.<br/>
    /// Networking activity on the local host has not been initiated.<br/>
    /// These conditions are more likely to be indicated by the error <see cref="WSAETIMEDOUT"/>.
    /// </summary>
    public const int WSAEHOSTDOWN = 10064;

    /// <summary>
    /// No route to host.<br/>
    /// A socket operation was attempted to an unreachable host.<br/>
    /// See <see cref="WSAENETUNREACH"/>.
    /// </summary>
    public const int WSAEHOSTUNREACH = 10065;

    /// <summary>
    /// Directory not empty.<br/>
    /// Cannot remove a directory that is not empty.
    /// </summary>
    public const int WSAENOTEMPTY = 10066;

    /// <summary>
    /// Too many processes.<br/>
    /// A Windows Sockets implementation may have a limit on the number of applications that can use it simultaneously.<br/>
    /// <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-wsastartup">WSAStartup</see> may fail with this error if the limit has been reached.
    /// </summary>
    public const int WSAEPROCLIM = 10067;

    /// <summary>
    /// User quota exceeded.<br/>
    /// Ran out of user quota.
    /// </summary>
    public const int WSAEUSERS = 10068;

    /// <summary>
    /// Disk quota exceeded.<br/>
    /// Ran out of disk quota.
    /// </summary>
    public const int WSAEDQUOT = 10069;

    /// <summary>
    /// Stale file handle reference.<br/>
    /// The file handle reference is no longer available.
    /// </summary>
    public const int WSAESTALE = 10070;

    /// <summary>
    /// Item is remote.<br/>
    /// The item is not available locally.
    /// </summary>
    public const int WSAEREMOTE = 10071;

    /// <summary>
    /// Network subsystem is unavailable.<br/>
    /// This error is returned by <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-wsastartup">WSAStartup</see> if the Windows Sockets implementation cannot function at this time because the underlying system it uses to provide network services is currently unavailable.<br/>
    /// Users should check:
    /// <list type="bullet">
    ///  <item>That the appropriate Windows Sockets DLL file is in the current path.</item>
    ///  <item>That they are not trying to use more than one Windows Sockets implementation simultaneously. If there is more than one Winsock DLL on your system, be sure the first one in the path is appropriate for the network subsystem currently loaded.</item>
    ///  <item>The Windows Sockets implementation documentation to be sure all necessary components are currently installed and configured correctly.</item>
    /// </list>
    /// </summary>
    public const int WSASYSNOTREADY = 10091;

    /// <summary>
    /// Winsock.dll version out of range.<br/>
    /// The current Windows Sockets implementation does not support the Windows Sockets specification version requested by the application.<br/>
    /// Check that no old Windows Sockets DLL files are being accessed.
    /// </summary>
    public const int WSAVERNOTSUPPORTED = 10092;

    /// <summary>
    /// Successful WSAStartup not yet performed.<br/>
    /// Either the application has not called <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-wsastartup">WSAStartup</see> or <b>WSAStartup</b> failed.<br/>
    /// The application may be accessing a socket that the current active task does not own (that is, trying to share a socket between tasks), or <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winsock/nf-winsock-wsacleanup">WSACleanup</see> has been called too many times.
    /// </summary>
    public const int WSANOTINITIALISED = 10093;

    /// <summary>
    /// Graceful shutdown in progress.<br/>
    /// Returned by <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-wsarecv">WSARecv</see> and <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-wsarecvfrom">WSARecvFrom</see> to indicate that the remote party has initiated a graceful shutdown sequence.
    /// </summary>
    public const int WSAEDISCON = 10101;

    /// <summary>
    /// No more results.<br/>
    /// No more results can be returned by the <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-wsalookupservicenexta">WSALookupServiceNext</see> function.
    /// </summary>
    public const int WSAENOMORE = 10102;

    /// <summary>
    /// Call has been canceled.<br/>
    /// A call to the <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-wsalookupserviceend">WSALookupServiceEnd</see> function was made while this call was still processing.<br/>
    /// The call has been cancelled.
    /// </summary>
    public const int WSAECANCELLED = 10103;

    /// <summary>
    /// Procedure call table is invalid.<br/>
    /// The service provider procedure call table is invalid.<br/>
    /// A service provider returned a bogus procedure table to Ws2_32.dll.<br/>
    /// This is usually caused by one or more of the function pointers being <b><see langword="null"/></b>.
    /// </summary>
    public const int WSAEINVALIDPROCTABLE = 10104;

    /// <summary>
    /// Service provider is invalid.<br/>
    /// The requested service provider is invalid.<br/>
    /// This error is returned by the <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Ws2spi/nf-ws2spi-wscgetproviderinfo">WSCGetProviderInfo</see> and <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Ws2spi/nf-ws2spi-wscgetproviderinfo32">WSCGetProviderInfo32</see> functions if the protocol entry specified could not be found.<br/>
    /// This error is also returned if the service provider returned a version number other than 2.0.
    /// </summary>
    public const int WSAEINVALIDPROVIDER = 10105;

    /// <summary>
    /// Service provider failed to initialize.<br/>
    /// The requested service provider could not be loaded or initialized.<br/>
    /// This error is returned if either a service provider's DLL could not be loaded (<see href="https://learn.microsoft.com/en-us/windows/desktop/api/libloaderapi/nf-libloaderapi-loadlibrarya">LoadLibrary</see> failed) or the provider's <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Ws2spi/nf-ws2spi-wspstartup">WSPStartup</see> or <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Ws2spi/nf-ws2spi-nspstartup">NSPStartup</see> function failed.
    /// </summary>
    public const int WSAEPROVIDERFAILEDINIT = 10106;

    /// <summary>
    /// System call failure.<br/>
    /// A system call that should never fail has failed.<br/>
    /// This is a generic error code, returned under various conditions.<br/>
    /// Returned when a system call that should never fail does fail.<br/>
    /// For example, if a call to <see href="">WaitForMultipleEvents</see> fails or one of the registry functions fails trying to manipulate the protocol/namespace catalogs.<br/>
    /// Returned when a provider does not return SUCCESS and does not provide an extended error code.<br/>
    /// Can indicate a service provider implementation error.
    /// </summary>
    public const int WSASYSCALLFAILURE = 10107;

    /// <summary>
    /// Service not found.<br/>
    /// No such service is known.<br/>
    /// The service cannot be found in the specified name space.
    /// </summary>
    public const int WSASERVICE_NOT_FOUND = 10108;

    /// <summary>
    /// Class type not found.<br/>
    /// The specified class was not found.
    /// </summary>
    public const int WSATYPE_NOT_FOUND = 10109;

    /// <summary>
    /// No more results.<br/>
    /// No more results can be returned by the <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-wsalookupservicenexta">WSALookupServiceNext</see> function.
    /// </summary>
    public const int WSA_E_NO_MORE = 10110;

    /// <summary>
    /// Call was canceled.<br/>
    /// A call to the <see href="https://learn.microsoft.com/en-us/windows/desktop/api/Winsock2/nf-winsock2-wsalookupserviceend">WSALookupServiceEnd</see> function was made while this call was still processing.<br/>
    /// The call has been canceled.
    /// </summary>
    public const int WSA_E_CANCELLED = 10111;

    /// <summary>
    /// Database query was refused.<br/>
    ///¨A database query failed because it was actively refused.
    /// </summary>
    public const int WSAEREFUSED = 10112;

    /// <summary>
    /// Host not found.<br/>
    /// No such host is known.<br/>
    /// The name is not an official host name or alias, or it cannot be found in the database(s) being queried.<br/>
    /// This error may also be returned for protocol and service queries, and means that the specified name could not be found in the relevant database.
    /// </summary>
    public const int WSAHOST_NOT_FOUND = 11001;

    /// <summary>
    /// Nonauthoritative host not found.<br/>
    /// This is usually a temporary error during host name resolution and means that the local server did not receive a response from an authoritative server.<br/>
    /// A retry some time later may be successful.
    /// </summary>
    public const int WSATRY_AGAIN = 11002;

    /// <summary>
    /// This is a nonrecoverable error.<br/>
    /// This indicates that some sort of nonrecoverable error occurred during a database lookup.<br/>
    /// This may be because the database files (for example, BSD-compatible HOSTS, SERVICES, or PROTOCOLS files) could not be found, or a DNS request was returned by the server with a severe error.
    /// </summary>
    public const int WSANO_RECOVERY = 11003;

    /// <summary>
    /// Valid name, no data record of requested type.<br/>
    /// The requested name is valid and was found in the database, but it does not have the correct associated data being resolved for.<br/>
    /// The usual example is a host name-to-address translation attempt (using <see href="https://learn.microsoft.com/en-us/windows/desktop/api/wsipv6ok/nf-wsipv6ok-gethostbyname">gethostbyname</see> or <see href="https://learn.microsoft.com/en-us/windows/desktop/api/wsipv6ok/nf-wsipv6ok-wsaasyncgethostbyname">WSAAsyncGetHostByName</see>) which uses the DNS (Domain Name Server).<br/>
    /// An MX record is returned but no A record---indicating the host itself exists, but is not directly reachable.
    /// </summary>
    public const int WSANO_DATA = 11004;

    /// <summary>
    /// QoS receivers.<br/>
    /// At least one QoS reserve has arrived.
    /// </summary>
    public const int WSA_QOS_RECEIVERS = 11005;

    /// <summary>
    /// QoS senders.<br/>
    /// At least one QoS send path has arrived.
    /// </summary>
    public const int WSA_QOS_SENDERS = 11006;

    /// <summary>
    /// No QoS senders.<br/>
    /// There are no QoS senders.
    /// </summary>
    public const int WSA_QOS_NO_SENDERS = 11007;

    /// <summary>
    /// QoS no receivers.<br/>
    /// There are no QoS receivers.
    /// </summary>
    public const int WSA_QOS_NO_RECEIVERS = 11008;

    /// <summary>
    /// QoS request confirmed.<br/>
    /// The QoS reserve request has been confirmed.
    /// </summary>
    public const int WSA_QOS_REQUEST_CONFIRMED = 11009;

    /// <summary>
    /// QoS admission error.<br/>
    /// A QoS error occurred due to lack of resources.
    /// </summary>
    public const int WSA_QOS_ADMISSION_FAILURE = 11010;

    /// <summary>
    /// QoS policy failure.<br/>
    /// The QoS request was rejected because the policy system couldn't allocate the requested resource within the existing policy.
    /// </summary>
    public const int WSA_QOS_POLICY_FAILURE = 11011;

    /// <summary>
    /// QoS bad style.<br/>
    /// An unknown or conflicting QoS style was encountered.
    /// </summary>
    public const int WSA_QOS_BAD_STYLE = 11012;

    /// <summary>
    /// QoS bad object.<br/>
    /// A problem was encountered with some part of the filterspec or the provider-specific buffer in general.
    /// </summary>
    public const int WSA_QOS_BAD_OBJECT = 11013;

    /// <summary>
    /// QoS traffic control error.<br/>
    /// An error with the underlying traffic control (TC) API as the generic QoS request was converted for local enforcement by the TC API.<br/>
    /// This could be due to an out of memory error or to an internal QoS provider error.
    /// </summary>
    public const int WSA_QOS_TRAFFIC_CTRL_ERROR = 11014;

    /// <summary>
    /// QoS generic error.<br/>
    /// A general QoS error.
    /// </summary>
    public const int WSA_QOS_GENERIC_ERROR = 11015;

    /// <summary>
    /// QoS service type error.<br/>
    /// An invalid or unrecognized service type was found in the QoS flowspec.
    /// </summary>
    public const int WSA_QOS_ESERVICETYPE = 11016;

    /// <summary>
    /// QoS flowspec error.<br/>
    /// An invalid or inconsistent flowspec was found in the <see href="https://learn.microsoft.com/en-us/windows/win32/api/winsock2/ns-winsock2-qos">QOS</see> structure.
    /// </summary>
    public const int WSA_QOS_EFLOWSPEC = 11017;

    /// <summary>
    /// Invalid QoS provider buffer.<br/>
    /// An invalid QoS provider-specific buffer.
    /// </summary>
    public const int WSA_QOS_EPROVSPECBUF = 11018;

    /// <summary>
    /// Invalid QoS filter style.<br/>
    /// An invalid QoS filter style was used.
    /// </summary>
    public const int WSA_QOS_EFILTERSTYLE = 11019;

    /// <summary>
    /// Invalid QoS filter type.<br/>
    /// An invalid QoS filter type was used.
    /// </summary>
    public const int WSA_QOS_EFILTERTYPE = 11020;

    /// <summary>
    /// Incorrect QoS filter count.<br/>
    /// An incorrect number of QoS FILTERSPECs were specified in the FLOWDESCRIPTOR.
    /// </summary>
    public const int WSA_QOS_EFILTERCOUNT = 11021;

    /// <summary>
    /// Invalid QoS object length.<br/>
    /// An object with an invalid ObjectLength field was specified in the QoS provider-specific buffer.
    /// </summary>
    public const int WSA_QOS_EOBJLENGTH = 11022;

    /// <summary>
    /// Incorrect QoS flow count.<br/>
    /// An incorrect number of flow descriptors was specified in the QoS structure.
    /// </summary>
    public const int WSA_QOS_EFLOWCOUNT = 11023;

    /// <summary>
    /// Unrecognized QoS object.<br/>
    /// An unrecognized object was found in the QoS provider-specific buffer.
    /// </summary>
    public const int WSA_QOS_EUNKOWNPSOBJ = 11024;

    /// <summary>
    /// Invalid QoS policy object.<br/>
    /// An invalid policy object was found in the QoS provider-specific buffer.
    /// </summary>
    public const int WSA_QOS_EPOLICYOBJ = 11025;

    /// <summary>
    /// Invalid QoS flow descriptor.<br/>
    /// An invalid QoS flow descriptor was found in the flow descriptor list.
    /// </summary>
    public const int WSA_QOS_EFLOWDESC = 11026;

    /// <summary>
    /// Invalid QoS provider-specific flowspec.<br/>
    /// An invalid or inconsistent flowspec was found in the QoS provider-specific buffer.
    /// </summary>
    public const int WSA_QOS_EPSFLOWSPEC = 11027;

    /// <summary>
    /// Invalid QoS provider-specific filterspec.<br/>
    /// An invalid FILTERSPEC was found in the QoS provider-specific buffer.
    /// </summary>
    public const int WSA_QOS_EPSFILTERSPEC = 11028;

    /// <summary>
    /// Invalid QoS shape discard mode object.<br/>
    /// An invalid shape discard mode object was found in the QoS provider-specific buffer.
    /// </summary>
    public const int WSA_QOS_ESDMODEOBJ = 11029;

    /// <summary>
    /// Invalid QoS shaping rate object.<br/>
    /// An invalid shaping rate object was found in the QoS provider-specific buffer.
    /// </summary>
    public const int WSA_QOS_ESHAPERATEOBJ = 11030;

    /// <summary>
    /// Reserved policy QoS element type.<br/>
    /// A reserved policy element was found in the QoS provider-specific buffer.
    /// </summary>
    public const int WSA_QOS_RESERVED_PETYPE = 11031;
}