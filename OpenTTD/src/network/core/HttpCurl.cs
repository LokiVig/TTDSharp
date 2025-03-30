using System;
using System.Collections.Generic;
using System.Threading;

namespace OpenTTD.Network.Core;

public static partial class NetworkCore
{
    /// <summary>
    /// List of certificate bundles, depending on OS. Taken from: https://golang.org/src/crypto/x509/root_unix.go
    /// </summary>
    public static string[] certificateFiles =
    {
        "/etc/ssl/certs/ca-certificates.crt", // Debian / Ubuntu / Gentoo etc.
        "/etc/pki/tls/certs/ca-bundle.crt", // Fedora / RHEL 6
	    "/etc/ssl/ca-bundle.pem", // OpenSUSE
	    "/etc/pki/tls/cacert.pem", // OpenELEC
	    "/etc/pki/ca-trust/extracted/pem/tls-ca-bundle.pem", // CentOS / RHEL 7
	    "/etc/ssl/cert.pem" // Alpine Linux
    };

    /// <summary>
    /// List of certificate directories, depending on OS. Taken from: https://golang.org/src/crypto/x509/root_unix.go
    /// </summary>
    public static string[] certificateDirectories =
    {
        "/etc/ssl/certs", // SLES10 / SLES11, https://golang.org/issue/12139
        "/etc/pki/tls/cers", // Fedora / RHEL
        "/system/etc/security/cacerts" // Android
    };

    public static List<HTTPThreadSafeCallback> httpCallbacks;
    public static List<HTTPThreadSafeCallback> newHttpCallbacks;
    public static Mutex httpCallbackMutex;
    public static Mutex newHttpCallbackMutex;

    public static Thread httpThread;
    public static bool httpThreadExit = false;
    public static Queue<NetworkHTTPRequest> httpRequests;
    public static Mutex httpMutex;
    public static string httpCaFile = "";
    public static string httpCaPath = "";

    public static void SetCurlOption( CURL curl, object option, object value )
    {
        throw new NotImplementedException( "CURL things don't necessarily exist..." );
    }

    public static void HttpThread()
    {
        throw new NotImplementedException( "CURL things don't necessarily exist..." );
    }
}

/// <summary>
/// Single HTTP request.
/// </summary>
public class NetworkHTTPRequest
{
    public string uri; // URI to connect to
    public HTTPThreadSafeCallback callback; // Callback to send data back on
    public string data; // Data to send, if any

    public NetworkHTTPRequest( string uri, HTTPCallback callback, string data )
    {
        this.uri = uri;
        this.callback = new HTTPThreadSafeCallback( callback );
        this.data = data;

        lock ( NetworkCore.newHttpCallbackMutex )
        {
            NetworkCore.newHttpCallbacks.Add( this.callback );
        }
    }

    ~NetworkHTTPRequest()
    {
        lock ( NetworkCore.httpCallbackMutex )
        {
            NetworkCore.httpCallbacks.Remove( callback );
        }
    }
}