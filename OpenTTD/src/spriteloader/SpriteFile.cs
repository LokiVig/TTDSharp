namespace OpenTTD.SpriteLoader;

/// <summary>
/// <see cref="RandomAccessFile"/> with some extra information specific for sprite files.<br/>
/// It automatically detects and stores the container version upload opening the file.
/// </summary>
public class SpriteFile : RandomAccessFile
{
    public static byte[] grfContV2Sig = { (byte)'G', (byte)'R', (byte)'F', 0x82, 0x0D, 0x1A, 0x0A };

    public bool paletteRemap; // Whether or not a remap of the palette is required for this file
    public byte containerVersion; // Container format of the sprite file
    public int contentBegin; // The begin of the content of the sprite file, i.e. after the container metadata

    public SpriteFile( string filename, SubDirectory subdir, bool paletteRemap )
    {
        base.RandomAccessFile( filename, subdir );

        this.paletteRemap = paletteRemap;
        containerVersion = GetGRFContainerVersion(this);
        contentBegin = GetPos();
    }

    /// <summary>
    /// Whether a palette remap is needed when loading sprites from this file.
    /// </summary>
    /// <returns><see langword="true"/> when needed, otherwise <see langword="false"/>.</returns>
    public bool NeedsPaletteRemap()
    {
        return paletteRemap;
    }

    /// <summary>
    /// Get the version number of container type used by the file.
    /// </summary>
    /// <returns>The version.</returns>
    public byte GetContainerVersion()
    {
        return containerVersion;
    }

    /// <summary>
    /// Seek to the beginning of the content, i.e. the position just after the container version has been determined.
    /// </summary>
    public void SeekToBegin()
    {
        SeekTo( contentBegin, Seek.Set );
    }

    public static byte GetGRFContainerVersion( SpriteFile file )
    {
        int pos = file.GetPos();

        if ( file.ReadWord() == 0 )
        {
            // Check for GRF container version 2, which is identified by the bytes
            // '47 52 46 82 0D 0A 1A 0A' at the start of the file
            foreach ( byte expectedSigByte in grfContV2Sig )
            {
                if ( file.ReadByte() != expectedSigByte )
                {
                    return 0; // Invalid format
                }
            }

            return 2;
        }

        // Container version 1 has no header, rewind to start
        file.SeekTo( pos, Seek.Set );
        return 1;
    }
}