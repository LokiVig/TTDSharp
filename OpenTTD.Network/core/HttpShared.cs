using System.Collections.Generic;
using System.Threading;
using System;

namespace OpenTTD.Network.Core;

/// <summary>
/// Converts a <see cref="HTTPCallback"/> to a thread-safe variant.
/// </summary>
public class HTTPThreadSafeCallback
{
    private HTTPCallback callback; // The callback to send data back on
    private Mutex mutex; // Mutex to protect the queue
    private Queue<Callback> queue; // Queue of data to send back

    public bool cancelled = false;

    private class Callback
    {
        public byte[] data;
        public ulong length;
        public bool failure;

        public Callback()
        {
            data = null;
            length = 0;
            failure = true;
        }

        public Callback( byte[] data, ulong length )
        {
            Array.Copy( data, this.data, data.Length );
            this.length = length;
            failure = false;
        }
    }

    public HTTPThreadSafeCallback( HTTPCallback callback )
    {
        this.callback = callback;
    }

    ~HTTPThreadSafeCallback()
    {
        lock ( mutex )
        {
            queue.Clear();
        }
    }

    /// <summary>
    /// Similar to <see cref="HTTPCallback.OnFailure"/>, but thread-safe.
    /// </summary>
    public void OnFailure()
    {
        lock ( mutex )
        {
            queue.Enqueue( new Callback() );
        }
    }

    /// <summary>
    /// Similar to <see cref="HTTPCallback.OnReceiveData(byte[], ulong)"/>, but thread-safe.
    /// </summary>
    public void OnReceiveData( byte[] data, ulong length )
    {
        lock ( mutex )
        {
            queue.Enqueue( new Callback( data, length ) );
        }
    }

    /// <summary>
    /// Process everything on the queue.<br/>
    /// Should be called from the Game Thread.
    /// </summary>
    public void HandleQueue()
    {
        cancelled = callback.IsCancelled();

        lock ( mutex )
        {
            foreach ( Callback item in queue )
            {
                if ( item.failure )
                {
                    callback.OnFailure();
                }
                else
                {
                    callback.OnReceiveData( item.data, item.length );
                }
            }

            queue.Clear();
        }
    }

    /// <summary>
    /// Wait till the queue is dequeued, or a condition is met.
    /// </summary>
    /// <param name="condition">Condition functor.</param>
    public void WaitTillEmptyOrCondition<T>( T condition )
    {
        lock ( mutex )
        {
            while ( !( queue.Count == 0 || condition() ) )
            {
                throw new NotImplementedException( "this->queue_cv.wait(lock)" );
            }
        }
    }

    /// <summary>
    /// Check if the queue is empty.
    /// </summary>
    public bool IsQueueEmpty()
    {
        lock ( mutex )
        {
            return queue.Count == 0;
        }
    }
}