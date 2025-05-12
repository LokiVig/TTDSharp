using System;
using System.Runtime.InteropServices;

namespace OpenTTD.Core;

public static class Alloc
{
    public static void MallocError( dynamic size )
    {
        throw new OutOfMemoryException( $"Out of memory. Cannot allocate {size} bytes." );
    }

    public static void ReallocError( dynamic size )
    {
        throw new OutOfMemoryException( $"Out of memory. Cannot reallocate {size} bytes." );
    }

    /// <summary>
    /// Checks whether allocating memory would overflow <see langword="dynamic"/>.
    /// </summary>
    /// <param name="elementSize">Size of the structure to allocate.</param>
    /// <param name="numElements">Number of elements to allocate.</param>
    public static void CheckAllocationConstraints( dynamic elementSize, dynamic numElements )
    {
        if ( numElements > Size.Max / elementSize )
        {
            MallocError( Size.Max );
        }
    }

    /// <summary>
    /// Checks whether allocating memory would overflow <see langword="dynamic"/>.
    /// </summary>
    /// <typeparam name="T">Structure to allocate.</typeparam>
    /// <param name="numElements">Number of elements to allocate.</param>
    public static void CheckAllocationConstraints<T>( dynamic numElements )
    {
        unsafe
        {
            CheckAllocationConstraints( sizeof( T ), numElements );
        }
    }

    /// <summary>
    /// Simplified allocation function that allocates the specified number of<br/>
    /// elements of the given type. It also explicitly casts it to the requested<br/>
    /// type.<br/>
    /// <br/>
    /// <para>
    ///     <b>NOTE:</b> Throws an error when there is no memory anymore.<br/>
    ///     <b>NOTE:</b> The memory contains garbage data (i.e. possibly non-zero values).
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the variable(s) to allocate.</typeparam>
    /// <param name="numElements">The number of elements to allocate of the given type.</param>
    /// <returns><see langword="null"/> when <c><paramref name="numElements"/> == 0</c>, non-<see langword="null"/> otherwise.</returns>
    public static T[] MallocT<T>( dynamic numElements )
    {
        // MorphOS cannot handle 0 element allocations, or rather that always
        // returns null, so we do that for *all* allocations, thus causing it
        // to behave the same on all OSes
        if ( numElements == 0 )
        {
            return null;
        }

        // Ensure the size does not overflow
        CheckAllocationConstraints<T>( numElements );

        T[] tPtr;

        unsafe
        {
            tPtr = Marshal.AllocHGlobal( numElements * sizeof( T ) );
        }

        if ( tPtr == null )
        {
            unsafe
            {
                MallocError( numElements * sizeof( T ) );
            }
        }

        return tPtr;
    }

    /// <summary>
    /// Simplified allocation function that allocates the specified number of<br/>
    /// elements of the given type. It also explicitly casts it to the requested<br/>
    /// type.<br/>
    /// <br/>
    /// <para>
    ///     <b>NOTE:</b> Throws an error when there is no memory anymore.<br/>
    ///     <b>NOTE:</b> The memory contains all zero values.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the variable(s) to allocation.</typeparam>
    /// <param name="numElements">The number of elements to allocate of the given type.</param>
    /// <returns><see langword="null"/> when <c><paramref name="numElements"/> == 0</c>, non-<see langword="null"/> otherwise.</returns>
    public static T[] CallocT<T>( dynamic numElements )
    {
        // MorphOS cannot handle 0 element allocation, or rather that always
        // returns null, so we do that for *all* allocations, thus causing it
        // to behave the same on all OSes
        if ( numElements == 0 )
        {
            return null;
        }

        T[] tPtr = Marshal.AllocCoTaskMem( numElements );

        if ( tPtr == null )
        {
            unsafe
            {
                MallocError( numElements * sizeof( T ) );
            }
        }

        return tPtr;
    }

    /// <summary>
    /// Simplified reallocation function that allocates the specified number of<br/>
    /// elements of the given type. It also explicitly casts it to the requested<br/>
    /// type. It extends / shrinks the memory allocation given in <paramref name="tPtr"/>.<br/>
    /// <br/>
    /// <para>
    ///     <b>NOTE:</b> Throws an error when there is no memory anymore.<br/>
    ///     <b>NOTE:</b> The pointer to the data may change, but the data will remain valid.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the variable(s) to allocation.</typeparam>
    /// <param name="tPtr">The previous allocation to extend / shrink.</param>
    /// <param name="numElements">The number of elements to allocate of the given type.</param>
    /// <returns><see langword="null"/> when <c><paramref name="numElements"/> == 0</c>, non-<see langword="null"/> otherwise.</returns>
    public static T[] ReallocT<T>( ref T[] tPtr, dynamic numElements )
    {
        // MorphOS cannot handle 0 element allocation, or rather that always
        // returns null, so we do that for *all* allocations, thus causing it
        // to behave the same on all OSes
        if ( numElements == 0 )
        {
            tPtr = null;
            return null;
        }

        // Ensure the size does not overflow
        CheckAllocationConstraints<T>( numElements );

        unsafe
        {
            //tPtr = Marshal.ReAllocHGlobal( tPtr, numElements * sizeof( T ) );
            throw new NotImplementedException( "Marshal.ReAllocHGlobal requires a nint input, T[] does not transfer to nint easily!" );
        }

        if ( tPtr == null )
        {
            unsafe
            {
                ReallocError( numElements * sizeof( T ) );
            }
        }

        return tPtr;
    }
}

/// <summary>
/// A reusable buffer that can be used for places that temporarily allocate<br/>
/// a bit of memory and do that very often, or for places where static memory<br/>
/// is allocated that might need to be reallocated sometimes.<br/>
/// <br/>
/// Every time <see cref="Allocate"/> or <see cref="ZeroAllocate"/> is called previous results<br/>
/// of both functions will become invalid.
/// </summary>
public class ReusableBuffer<T>
{
    private T[] buffer; // The real data buffer
    private int count; // Number of T elements in the buffer

    /// <summary>
    /// Create a new buffer.
    /// </summary>
    public ReusableBuffer()
    {
        buffer = null;
        count = 0;
    }

    /// <summary>
    /// Clear the buffer
    /// </summary>
    ~ReusableBuffer()
    {
        buffer = null;
        count = 0;
    }

    /// <summary>
    /// Get buffer of at least count times T.<br/>
    /// <br/>
    /// <para>
    ///     <b>NOTE:</b> The buffer might be bigger.<br/>
    ///     <b>NOTE:</b> Calling this function invalidates any previous buffers given.<br/>
    /// </para>
    /// </summary>
    /// <param name="count">The minimum buffer size.</param>
    /// <returns>The buffer.</returns>
    public T[] Allocate( int count )
    {
        if ( this.count < count )
        {
            buffer = null;
            buffer = Alloc.MallocT<T>( count );
            this.count = count;
        }

        return buffer;
    }

    /// <summary>
    /// Get buffer of at least count times T with zeroed memory.<br/>
    /// <br/>
    /// <para>
    ///     <b>NOTE:</b> The buffer might be bigger.<br/>
    ///     <b>NOTE:</b> Calling this function invalidates any previous buffers given.<br/>
    /// </para>
    /// </summary>
    /// <param name="count">The minimum buffer size.</param>
    /// <returns>The buffer.</returns>
    public T[] ZeroAllocate( int count )
    {
        if ( this.count < count )
        {
            buffer = null;
            buffer = Alloc.CallocT<T>( count );
            this.count = count;
        }
        else
        {
            unsafe
            {
                buffer = new T[sizeof( T ) * count];
            }
        }

        return buffer;
    }

    /// <summary>
    /// Get the currently allocated buffer.
    /// </summary>
    /// <returns>The buffer.</returns>
    public T[] GetBuffer()
    {
        return buffer;
    }
}

/// <summary>
/// Base class that provides memory initialization on dynamically created objects.<br/>
/// All allocated memory will be zeroed.
/// </summary>
public class ZeroedMemoryAllocator
{
    protected byte[] buffer;

    public ZeroedMemoryAllocator( int size )
    {
        buffer = Alloc.CallocT<byte>( size );
    }

    ~ZeroedMemoryAllocator()
    {
        buffer = null;
    }
}