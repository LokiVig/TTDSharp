global using PacketSize = ushort;
global using PacketType = byte;

using System;
using System.Diagnostics;

namespace OpenTTD.Network.Core;

/// <summary>
/// Internal entity of a packet. As everything is sent as a packet,<br/>
/// all network communication will need to call the functions that<br/>
/// populate the packet.<br/>
/// Every packet can be at most a limited number bytes set in the<br/>
/// constructor. Overflowing this limit will give an assertion when<br/>
/// sending (i.e. writing) the packet. Reading past the size of the<br/>
/// packet when receiving will return all 0 values and "" in case of<br/>
/// the string.<br/>
/// <br/>
/// --- Points of attention ---<br/>
///     - All > 1 byte integral values are written in little endian,<br/>
///     unless specified otherwise.<br/>
///         Thus, <c>0x01234567</c> would be sent as <c>{0x67, 0x45, 0x23, 0x01}</c>.<br/>
/// <br/>
///     - All sent strings are of variable length and not terminated by a <c>'\0'</c>.<br/>
///         Thus, the length of the strings is not sent.<br/>
/// <br/>
///     - Years that are leap years in the 'days since X' to 'date' calculations:<br/>
///     <c>(year % 4 == 0)</c> and <c>((year % 100 != 0) or (year % 400 == 0))</c>.
/// </summary>
public struct Packet
{
    // The current read / write position in the packet
    private PacketSize pos;

    // The buffer of this packet
    private byte[] buffer;

    // The limit for the packet size
    private dynamic limit;

    // Socket we're associated with
    private NetworkSocketHandler cs;

    public Packet( NetworkSocketHandler cs, int limit, int initialReadSize = sizeof(PacketSize) /* EncodedLengthOfPacketSize() */ )
    {

    }

    public Packet( NetworkSocketHandler cs, PacketType type, int limit = Config.COMPAT_MTU )
    {

    }

    public static ulong EncodedLengthOfPacketSize()
    {
        return sizeof( PacketSize );
    }

    public static ulong EncodedLengthOfPacketType()
    {
        return sizeof( PacketType );
    }

    public void PrepareToSend()
    {

    }

    public bool CanWriteToPacket( ulong bytesToWrite )
    {

    }

    public void SendBool( bool data )
    {

    }

    public void SendByte( byte data )
    {

    }

    public void SendUShort( ushort data )
    {

    }

    public void SendUInt( uint data )
    {

    }

    public void SendULong( ulong data )
    {

    }

    public void SendString( string data )
    {

    }

    public void SendBuffer( byte[] data )
    {

    }

    public byte[] SendBytes( byte[] span )
    {

    }

    public bool HasPacketSizeData()
    {

    }

    public bool ParsePacketSize()
    {

    }

    public ulong Size()
    {

    }

    public bool PrepareToRead()
    {

    }

    public PacketType GetPacketType()
    {

    }

    public bool CanReadFromPacket( ulong bytesToRead, bool closeConnection = false )
    {

    }

    public bool RecvBool()
    {

    }

    public byte RecvByte()
    {

    }

    public ushort RecvUShort()
    {

    }

    public uint RecvUInt()
    {

    }

    public ulong RecvULong()
    {

    }

    public byte[] RecvBuffer()
    {

    }

    public ulong RecvBytes( byte[] span )
    {

    }

    public string RecvString( ulong length, StringValidationSettings settings = StringValidationSettings.ReplaceWithQuestionMark )
    {

    }

    public ulong RemainingBytesToTransfer()
    {

    }

    /// <summary>
    /// Transfer data from the packet to the given function. It starts reading at the<br/>
    /// position the last transfer stopped.<br/>
    /// See <see cref="TransferIn{T}(Func{T, PacketType[], ulong, ulong}, T)"/> for more information about transferring data to functions.
    /// </summary>
    /// <param name="transferFunction">The function to pass the buffer as second parameter and the<br/>
    ///                                amount to write as the third parameter. It returns the amount that<br/>
    ///                                was written or -1 upon errors.</param>
    /// <param name="limit">The maximum amount of bytes to transfer.</param>
    /// <param name="destination">The first parameter of <paramref name="transferFunction"/>.</param>
    /// <returns>The return value of the <paramref name="transferFunction"/>.</returns>
    public long TransferOutWithLimit<T>( Func<T, byte[], ulong, ulong> transferFunction, long limit, T destination )
    {
        ulong amount = Math.Min( RemainingBytesToTransfer(), (ulong)limit );
        
        if ( amount == 0 )
        {
            return 0;
        }

        Debug.Assert( pos < buffer.Length );
        Debug.Assert( (int)(pos + amount) <= buffer.Length );

        byte[] outputBuffer = buffer[pos..(int)( pos + amount )];
        long bytes = (long)transferFunction( destination, outputBuffer, amount );

        if ( bytes > 0 )
        {
            pos += (ushort)bytes;
        }

        return bytes;
    }

    /// <summary>
    /// Transfer data from the packet to the given function. It starts reading at the<br/>
    /// position the last transfer stopped.<br/>
    /// See <see cref="TransferIn{T}(Func{T, PacketType[], ulong, ulong}, T)"/> for more information about transferring data to functions.
    /// </summary>
    /// <param name="transferFunction">The function to pass the buffer as second parameter and the amount<br/>
    ///                                to write as the third parameter. It returns the amount that<br/>
    ///                                was written or -1 upon errors.</param>
    /// <param name="destination">The first parameter of the <paramref name="transferFunction"/>.</param>
    /// <returns></returns>
    public long TransferOut<T>( Func<T, byte[], ulong, ulong> transferFunction, T destination )
    {
        return TransferOutWithLimit( transferFunction, long.MaxValue, destination );
    }

    /// <summary>
    /// Transfer data from the given function into the packet. It starts writing at the<br/>
    /// position the last transfer stopped.<br/>
    /// <br/>
    /// Examples of functions that can be used to transfer data into a packet are TCP's<br/>
    /// Recv and UDP's RecvFrom functions. They will directly write their data into the<br/>
    /// packet without an intermediate buffer.<br/>
    /// Examples of functions that can be used to transfer data from a packet are TCP's<br/>
    /// Send and UPD's SendTo functions. They will directly read the data from the packet's<br/>
    /// buffer without an intermediate buffer.<br/>
    /// These are functions that are special in a sense as even though the packet can send or<br/>
    /// receive an amount of data, those functions can say they only processed a smaller<br/>
    /// amount, so special handling is required to keep the position pointers correct.<br/>
    /// Most of these transfer functions are in the form of <c>function(source, buffer, amount, ...)</c>,<br/>
    /// so the template of this function will assume that as the base parameter order.<br/>
    /// <br/>
    /// This will attempt to write to all the remaining bytes into the packet. It updates the<br/>
    /// position based on how many bytes were actually written by the called <paramref name="transferFunction"/>.
    /// </summary>
    /// <param name="transferFunction">The function to pass the buffer as second parameter and the<br/>
    ///                                amount to read as third parameter. It returns the amount that<br/>
    ///                                was read or -1 upon errors.</param>
    /// <param name="source">The first parameter of the <paramref name="transferFunction"/>.</param>
    /// <returns>The return value of the <paramref name="transferFunction"/>.</returns>
    public long TransferIn<T>( Func<T, byte[], ulong, ulong> transferFunction, T source )
    {
        ulong amount = RemainingBytesToTransfer();

        if ( amount == 0 )
        {
            return 0;
        }

        Debug.Assert( pos < buffer.Length );
        Debug.Assert( (int)( pos + amount ) <= buffer.Length );

        byte[] inputBuffer = buffer[pos..(int)( pos + amount )];
        ulong bytes = transferFunction( source, inputBuffer, amount );

        if ( bytes > 0 )
        {
            pos += (ushort)bytes;
        }

        return (long)bytes;
    }
}