using System;
using System.Diagnostics;
using System.IO;

namespace OpenTTD;

/// <summary>
/// A file from which bytes, words and doubles are read in (potentially) a random order.<br/>
/// <br/>
/// This is mostly intended to be used for things that can be read from GRFs when needed, so<br/>
/// the graphics but also the sounds. This also ties into the spritecache as it uses these<br/>
/// files to load the sprites from when needed.
/// </summary>
public class RandomAccessFile
{
    private const int BUFFER_SIZE = 512;

    private string filename; // Full name of the file; relative path to subdir plus the extension of the file
    private string simplifiedFilename; // Simplified lowercase name of the file; only the name, no path or extension

    private FileStream? fileHandle; // File handle of the open file
    private dynamic pos; // Position in the file of the end of the read buffer
    private dynamic startPos; // Start position of the file, may be non-zero if file is within a tar file
    private dynamic endPos; // End position of file

    private byte[] buffer; // Current position within the local buffer
    private byte[] bufferEnd; // Last valid byte of buffer
    private byte[] bufferStart = new byte[BUFFER_SIZE]; // Local buffer when read from file

    private int bufferIndex;

    public RandomAccessFile( string filename, Subdirectory subdir )
    {
        fileHandle = File.Open( filename, FileMode.Open );
        dynamic fileSize = fileHandle.Length;

        if ( fileHandle == null )
        {
            throw new FileNotFoundException( $"Cannot open file \"{filename}\"" );
        }

        // When files are in a tar-file, the begin of the file might not be at 0
        long pos = fileHandle.Position;

        if ( pos < 0 )
        {
            throw new FileLoadException( $"Cannot read file \"{filename}\"" );
        }

        // Make a note of start and end position for readers who check bounds
        startPos = pos;
        endPos = startPos + fileSize;

        int t = filename.LastIndexOf( Path.DirectorySeparatorChar );
        string nameWithoutPath = filename[( t != -1 ? t + 1 : 0 )..];

        simplifiedFilename = nameWithoutPath[..nameWithoutPath.LastIndexOf( '.' )];
        simplifiedFilename = simplifiedFilename.ToLower();

        SeekTo( pos, Seek.Set );
    }

    /// <summary>
    /// Get the filename of the opened file with the path from the SubDirectory and the extension.
    /// </summary>
    /// <returns>Name of the file.</returns>
    public string GetFileName()
    {
        return filename;
    }

    /// <summary>
    /// Get the simplified filename of the opened file. The simplified filename is the name of the<br/>
    /// file without the SubDirectory or extension in lower case.
    /// </summary>
    /// <returns>Name of the file.</returns>
    public string GetSimplifiedFileName()
    {
        return simplifiedFilename;
    }

    /// <summary>
    /// Get position in the file.
    /// </summary>
    /// <returns>Position in the file.</returns>
    public dynamic GetPos()
    {
        return pos + ( buffer[bufferIndex] - bufferEnd[bufferIndex] );
    }

    public dynamic GetStartPos()
    {
        return startPos;
    }

    public dynamic GetEndPos()
    {
        return endPos;
    }

    /// <summary>
    /// Seek in the current file.
    /// </summary>
    /// <param name="pos">New position.</param>
    /// <param name="mode">Type of seek (<c><see cref="Seek.Cur"/></c> means <paramref name="pos"/> is relative to current position, <c><see cref="Seek.Set"/></c> means <paramref name="pos"/> is absolute).</param>
    public void SeekTo( dynamic pos, int mode )
    {
        if ( mode == (int)Seek.Cur )
        {
            pos += GetPos();
        }

        this.pos = pos;

        if ( fileHandle.ReadByte() < 0 )
        {
            throw new NotImplementedException("Function \"Debug\" is not yet implemented.");
        }

        // Reset the buffer, so the next ReadByte will read bytes from the file
        buffer = bufferEnd = bufferStart;
    }

    /// <summary>
    /// Test if we have reached the end of the file.
    /// </summary>
    /// <returns><see langword="true"/> if the current position as at or after the end of the file.</returns>
    public bool AtEndOfFile()
    {
        return GetPos() >= GetEndPos();
    }

    /// <summary>
    /// Read a byte from the file.
    /// </summary>
    /// <returns>Read byte.</returns>
    public byte ReadByte()
    {
        if ( buffer[bufferIndex] == bufferEnd[bufferIndex] )
        {
            buffer[bufferIndex] = bufferStart[bufferIndex];

            dynamic size = fileHandle.Read( buffer, 1, BUFFER_SIZE );
            pos += size;
            bufferEnd[bufferIndex] = bufferStart[bufferIndex] + size;

            if ( size == 0 )
            {
                return 0;
            }
        }

        return buffer[bufferIndex++];
    }

    /// <summary>
    /// Read a word (16 bits) from the file (in low endian format).
    /// </summary>
    /// <returns>Read word.</returns>
    public ushort ReadWord()
    {
        byte b = ReadByte();
        return (ushort)(( ReadByte() << 8 ) | b);
    }

    /// <summary>
    /// Read a double word (32 bits) from the file (in low endian format).
    /// </summary>
    /// <returns>Read word.</returns>
    public uint ReadDword()
    {
        uint b = ReadWord();
        return (uint)(ReadWord() << 16) | b;
    }


    /// <summary>
    /// Read a block.
    /// </summary>
    /// <param name="ptr">Destination buffer.</param>
    /// <param name="size">Number of bytes to read.</param>
    public void ReadBlock( ref byte[] dst, dynamic size )
    {
        if ( buffer[bufferIndex] != bufferEnd[bufferIndex] )
        {
            dynamic toCopy = Math.Min( size, bufferEnd[bufferIndex] - buffer[bufferIndex] );
            Array.Copy( dst, buffer, toCopy );
            bufferIndex += toCopy;
            size -= toCopy;

            if ( size == 0 )
            {
                return;
            }

            // ptr = static_cast<char*>(ptr) + toCopy;
            //dst = 
        }

        pos += fileHandle.Read( dst, 1, size );
    }

    /// <summary>
    /// Skip <paramref name="n"/> bytes ahead in the file.
    /// </summary>
    /// <param name="n">Number of bytes to skip reading.</param>
    public void SkipBytes( dynamic n )
    {
        Debug.Assert( bufferEnd[bufferIndex] >= buffer[bufferIndex] );

        dynamic remaining = bufferEnd[bufferIndex] - buffer[bufferIndex];

        if ( n <= remaining )
        {
            bufferIndex += n;
        }
        else
        {
            SeekTo( n, Seek.Cur );
        }
    }
}