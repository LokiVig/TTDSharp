using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace OpenTTD.Network.Core;

/// <summary>
/// Callback for when the HTTP handler has something to tell us.
/// </summary>
public class HTTPCallback
{
    /// <summary>
    /// An error has occurred and the connection has been closed.<br/>
    /// <b>NOTE:</b> HTTP socket handler is closed/freed.
    /// </summary>
    public virtual void OnFailure() { }

    /// <summary>
    /// We're receiving data.<br/>
    /// <b>NOTE:</b> When <see langword="null"/> is sent the HTTP socket handler is closed/freed.
    /// </summary>
    /// <param name="data">The received data, <see langword="null"/> when all data has been received.</param>
    /// <param name="length">The amount of received data, 0 when all data has been received.</param>
    public virtual void OnReceiveData( byte[] data, ulong length ) { }

    /// <summary>
    /// Check if there's a request to cancel the transfer.<br/>
    /// <b>NOTE:</b> Cancellations are never instant, and can take a bit of time to be processed.<br/>
    /// The object needs to remain valid until the <see cref="OnFailure"/> callback is called.
    /// </summary>
    /// <returns><see langword="true"/> if the connection is cancelled.</returns>
    public virtual bool IsCancelled() => false;
}

/// <summary>
/// Base socket handler for HTTP traffic.
/// </summary>
public class NetworkHTTPSocketHandler
{
    /// <summary>
    /// Connect to the given URI.
    /// </summary>
    /// <param name="uri">The URI to connect to (https://.../..).</param>
    /// <param name="callback">The callback to send data back on.</param>
    /// <param name="data">The data we want to send. When non-empty, this will be a POST request, otherwise a GET request.</param>
    public static void Connect( string uri, ref HTTPCallback callback, string data = "" )
    {
        if ( string.IsNullOrEmpty( Core.httpCaFile ) && string.IsNullOrEmpty( Core.httpCaPath ) )
        {
            callback.OnFailure();
            return;
        }

        lock ( Core.httpMutex )
        {
            Core.httpRequests.Enqueue( new NetworkHTTPRequest( uri, callback, data ) );
        }
    }

    /// <summary>
    /// Do the receiving for all HTTP connections.
    /// </summary>
    public static void HTTPReceive()
    {
        lock ( Core.httpCallbackMutex )
        {
            lock ( Core.newHttpCallbackMutex )
            {
                if ( Core.newHttpCallbacks.Count != 0 )
                {
                    // We delay adding new callbacks, as HandleQueue() below might add a new callback
                    Core.httpCallbacks.Insert( Core.httpCallbacks.Count, Core.newHttpCallbacks[0] );
                    Core.newHttpCallbacks.Clear();
                }
            }
        }

        foreach ( HTTPThreadSafeCallback callback in Core.httpCallbacks )
        {
            callback.HandleQueue();
        }
    }
}

public static partial class Core
{
    /// <summary>
    /// Initialize the HTTP socket handler.
    /// </summary>
    public static void NetworkHTTPInitialize() 
    {
        throw new NotImplementedException( "CURL things don't necessarily exist..." );
    }

    /// <summary>
    /// Uninitialize the HTTP socket handler.
    /// </summary>
    public static void NetworkHTTPUninitialize()
    {
        httpThreadExit = true;

        // Ensure the callbacks are handled, this is mostly needed as we send
        // a survey just before close, and that might be pending here
        NetworkHTTPSocketHandler.HTTPReceive();

        lock ( httpMutex )
        {

        }

        throw new NotImplementedException( "CURL things don't necessarily exist..." );
    }
}