global using SpriteCollection = OpenTTD.SpriteLoader.SpriteLoader.Sprite[];
global using SpriteComponents = OpenTTD.Core.EnumBitSet<OpenTTD.SpriteLoader.SpriteComponent, byte, byte>;

using System;
using System.Runtime.CompilerServices;

using OpenTTD.Core;

namespace OpenTTD.SpriteLoader;

/// <summary>
/// The different colour components a sprite can have.
/// </summary>
public enum SpriteComponent : byte
{
    RGB = 0, // Sprite has RGB
    Alpha = 1, // Sprite has alpha
    Palette = 2, // Sprite has palette data
    End,
}

/// <summary>
/// Interface for the loader of our sprites.
/// </summary>
public class SpriteLoader
{
    /// <summary>
    /// Definition of a common pixel in OpenTTD's realm.
    /// </summary>
    public struct CommonPixel
    {
        public byte r; // Red channel
        public byte g; // Green channel
        public byte b; // Blue channel
        public byte a; // Alpha channel
        public byte m; // Remap channel
    }

    /// <summary>
    /// Structure for passing information from the sprite loader to the blitter.<br/>
    /// You can only use this struct once at a time when using <see cref="AllocateData(ZoomLevel, int)"/> to<br/>
    /// allocate the memory as that will always return the same memory address.<br/>
    /// This is to prevent thousands of malloc + free's just to load a sprite.
    /// </summary>
    public struct Sprite
    {
        public ushort height; // Height of the sprite
        public ushort width; // Width of the sprite
        public short xOffs; // The X offset of where the sprite will be drawn
        public short yOffs; // The Y offset of where the sprite will be drawn
        public SpriteType type; // The sprite type
        public SpriteComponent colours; // The colour components of the sprite with useful information
        public CommonPixel[] data; // The sprite itself

        // Allocated memory to pass sprite data around
        private static ReusableBuffer<CommonPixel>[] buffer = new ReusableBuffer<CommonPixel>[(int)ZoomLevel.End];

        /// <summary>
        /// Allocate the sprite data of this sprite.
        /// </summary>
        /// <param name="zoom">The zoom level to allocate the data for.</param>
        /// <param name="size">The minimum size of the data field.</param>
        public void AllocateData( ZoomLevel zoom, int size )
        {
            data = buffer[(int)zoom].ZeroAllocate( size );
        }
    }

    /// <summary>
    /// Load a sprite from the disk and return a sprite struct which is the same for all loaders.
    /// </summary>
    /// <param name="sprite">The sprites to fill with data.</param>
    /// <param name="file">The file "descriptor" of the file we read from.</param>
    /// <param name="filePos">The position within the file the image begins.</param>
    /// <param name="spriteType">The type of sprite we're trying to load.</param>
    /// <param name="load32bpp"><see langword="true"/> if 32bpp sprites should be loaded, <see langword="false"/> for a 8bpp sprite.</param>
    /// <param name="controlFlags">Control flags, see <see cref="SpriteCacheCtrlFlags"/>.</param>
    /// <returns>Bit mask of the zoom levels successfully loaded, or 0 if no sprite could be loaded.</returns>
    public virtual byte LoadSprite( ref SpriteCollection sprite, SpriteFile file, int filePos, SpriteType spriteType, bool load32bpp, byte controlFlags, byte avail8bpp, byte avail32bpp )
    {
        return 0;
    }

    public static void Convert32bppTo8bpp( Sprite sprite )
    {
        CommonPixel pixelEnd = sprite.data[sprite.width * sprite.height];

        for ( int i = 0; i < sprite.data.Length; i++ )
        {
            CommonPixel pixel = sprite.data[i];

            if ( pixel.m != 0 )
            {
                // Pixel has 8bpp mask, test if should be reshaded
                byte brightness = Math.Max( Math.Max( pixel.r, pixel.g ), Math.Max( pixel.g, pixel.b ) );

                if ( brightness == 0 || brightness == 128 )
                {
                    continue;
                }

                // Update RGB component with reshaded palette colour, and enabled reshade
                Colour c = AdjustBrightness( curPalette.palette[pixel.m], brightness );

                if ( IsInsideMM( pixel.m, 0xC6, 0xCE ) )
                {
                    // Dumb but simple brightness conversion
                    pixel.m = GetNearestColourReshadeIndex( ( c.r + c.g + c.b ) / 3 );
                }
                else
                {
                    pixel.m = GetNearestColourIndex( c.r, c.g, c.b );
                }
            }
            else if ( pixel.a < 128 )
            {
                // Transparent pixel
                pixel.m = 0;
            }
            else
            {
                // Find nearest match from palette
                pixel.m = GetNearestColourIndex( pixel.r, pixel.g, pixel.b );
            }
        }
    }

    public static bool WarnCorruptSprite( SpriteFile file, int filePos, [CallerLineNumber] int line = 0 )
    {
        //byte warningLevel = 0;

        //if ( warningLevel == 0 )
        //{

        //}

        throw new NotImplementedException( "Not yet implemented!" );
    }

    /// <summary>
    /// Decode the image data of a single sprite.
    /// </summary>
    /// <param name="sprite">Filled with the sprite image data.</param>
    /// <param name="file">The file with the sprite data.</param>
    /// <param name="filePos">File position.</param>
    /// <param name="spriteType">Type of the sprite we're decoding.</param>
    /// <param name="num">Size of the decompressed sprite.</param>
    /// <param name="type">Type of the encoded sprite.</param>
    /// <param name="zoomLvl">Requested zoom level.</param>
    /// <param name="colourFmt">Colour format of the sprite.</param>
    /// <param name="containerFormat">Container format of the GRF this sprite is in.</param>
    /// <returns><see langword="true"/> if the sprite was successfully loaded.</returns>
    public static bool DecodeSingleSprite( ref Sprite sprite, SpriteFile file, int filePos, SpriteType spriteType, long num, byte type, ZoomLevel zoomLvl, SpriteComponent colourFmt, byte containerFormat )
    {
        // Original sprite height was max 255 pixels, with 4x extra zoom => 1020 pixels
        // Original maximum width for sprites was 640 pixels, with 4x extra zoom => 2560 pixels
        // Now up to 5 bytes per pixel => 1020 * 2560 * 5 => 12.5 MiB
        //
        // So, any sprite data more than 64 MiB is way larger than we would even expect; prevent allocating more memory!
        if ( num < 0 || num > 64 * 1024 * 1024 )
        {
            return WarnCorruptSprite( file, filePos );
        }

        byte[] destOrig = new byte[num];
        byte[] dest = destOrig;
        int destIndex = 0;
        long destSize = num;

        // Read the file, which has some kind of compression
        while ( num > 0 )
        {
            byte code = file.ReadByte();

            if ( code >= 0 )
            {
                // Plain bytes to read
                int size = ( code == 0 ) ? 0x80 : code;
                num -= size;

                if ( num < 0 )
                {
                    return WarnCorruptSprite( file, filePos );
                }

                for ( ; size > 0; size-- )
                {
                    dest[destIndex++] = file.ReadByte();
                }
            }
            else
            {
                // Copy bytes from earlier in the sprite
                uint dataOffset = (uint)( ( code & 7 ) << 8 ) | file.ReadByte();

                if ( dest[destIndex] - dataOffset < destOrig[destIndex] )
                {
                    return WarnCorruptSprite( file, filePos );
                }

                int size = -( code >> 3 );
                num -= size;

                if ( num < 0 )
                {
                    return WarnCorruptSprite( file, filePos );
                }

                for ( ; size > 0; size-- )
                {
                    dest[destIndex++] = (byte)( dest[destIndex] - dataOffset );
                }
            }
        }

        if ( num != 0 )
        {
            return WarnCorruptSprite( file, filePos );
        }

        sprite.AllocateData( zoomLvl, sprite.width * sprite.height );

        // Convert colour depth to pixel size
        int bpp = 0;

        if ( colourFmt.Test( SpriteComponent.RGB ) ) // Has RGB data
        {
            bpp += 3;
        }

        if ( colourFmt.Test( SpriteComponent.Alpha ) ) // Has alpha datas
        {
            bpp++;
        }

        if ( colourFmt.Test( SpriteComponent.Palette ) ) // Has palette data
        {
            bpp++;
        }

        // When there are transparency pixels, this format has another trick... Decode it
        if ( ( type & 0x80 ) != 0 )
        {
            for ( int y = 0; y < sprite.height; y++ )
            {
                bool lastItem = false;

                // Look up in the header table where the real data is stored for this row
                int offset;

                if ( containerFormat >= 2 && destSize > ushort.MaxValue )
                {
                    offset = ( destOrig[y * 4 + 3] << 24 ) | ( destOrig[y * 4 + 2] << 16 ) | ( destOrig[y * 4 + 1] << 8 ) | destOrig[y * 4];
                }
                else
                {
                    offset = ( destOrig[y * 2 + 1] << 8 ) | destOrig[y * 2];
                }

                // Go to that row
                dest[destIndex] = (byte)( destOrig[destIndex] + offset );

                do
                {
                    if ( dest[destIndex] + ( containerFormat >= 2 && sprite.width > 256 ? 4 : 2 ) > destOrig[destIndex] + destSize )
                    {
                        return WarnCorruptSprite( file, filePos );
                    }

                    CommonPixel[] data = new CommonPixel[sprite.data.Length];
                    int dataIndex = 0;

                    // Read the header
                    int length, skip;

                    if ( containerFormat >= 2 && sprite.width > 256 )
                    {
                        // 0  .. 14 - length
                        // 15       - lastItem
                        // 16 .. 31 - transparency bytes
                        lastItem = ( dest[destIndex + 1] & 0x80 ) != 0;
                        length = ( ( dest[destIndex + 1] & 0x7F ) << 8 ) | dest[destIndex + 0];
                        skip = ( dest[destIndex + 3] << 8 ) | dest[destIndex + 2];
                        destIndex += 4;
                    }
                    else
                    {
                        // 0 ..  6 - length
                        // 7       - lastItem
                        // 8 .. 15 - transparency bytes
                        lastItem = ( dest[destIndex] & 0x80 ) != 0;
                        length = ( dest[destIndex++] ) & 0x7F;
                        skip = dest[destIndex++];
                    }

                    data[dataIndex] = sprite.data[y * sprite.width + skip];

                    if ( skip + length > sprite.width || dest[destIndex] + length * bpp > destOrig[destIndex] + destSize )
                    {
                        return WarnCorruptSprite( file, filePos );
                    }

                    for ( int x = 0; x < length; x++ )
                    {
                        if ( colourFmt.Test( SpriteComponent.RGB ) )
                        {
                            data[dataIndex].r = dest[destIndex++];
                            data[dataIndex].g = dest[destIndex++];
                            data[dataIndex].b = dest[destIndex++];
                        }

                        data[dataIndex].a = colourFmt.Test( SpriteComponent.Alpha ) ? dest[destIndex++] : 0xFF;

                        if ( colourFmt.Test( SpriteComponent.Palette ) )
                        {
                            switch ( spriteType )
                            {
                                case SpriteType.Normal:
                                    data[dataIndex].m = file.NeedsPaletteRemap() ? palmapW2D[dest[destIndex]] : dest[destIndex];
                                    break;

                                case SpriteType.Font:
                                    data[dataIndex].m = (byte)Math.Min( dest[destIndex], 2u );
                                    break;

                                default:
                                    data[dataIndex].m = dest[destIndex];
                                    break;
                            }

                            // Magic blue
                            if ( colourFmt == SpriteComponent.Palette && dest[destIndex] == 0 )
                            {
                                data[dataIndex].a = 0x00;
                            }

                            destIndex++;
                        }

                        dataIndex++;
                    }
                } while ( !lastItem );
            }
        }
        else
        {
            long spriteSize = sprite.width * sprite.height * bpp;

            if ( destSize < spriteSize )
            {
                return WarnCorruptSprite( file, filePos );
            }

            if ( destSize > spriteSize )
            {
                byte warningLevel = 0;
                throw new NotImplementedException( "Function \"Debug\" is not yet implemented!" );
            }

            dest = destOrig;

            for ( int i = 0; i < sprite.width * sprite.height; i++ )
            {
                byte pixel = dest[i * bpp];

                if ( colourFmt.Test( SpriteComponent.RGB ) )
                {
                    sprite.data[i].r = pixel++;
                    sprite.data[i].g = pixel++;
                    sprite.data[i].b = pixel++;
                }

                sprite.data[i].a = colourFmt.Test( SpriteComponent.Alpha ) ? pixel++ : 0xFF;

                if ( colourFmt.Test( SpriteComponent.Palette ) )
                {
                    switch ( spriteType )
                    {
                        case SpriteType.Normal:
                            sprite.data[i].m = file.NeedsPaletteRemap() ? palmapW2D[pixel] : pixel;
                            break;

                        case SpriteType.Font:
                            sprite.data[i].m = (byte)Math.Min( pixel, 2u );
                            break;

                        default:
                            sprite.data[i].m = pixel;
                            break;
                    }

                    // Magic blue
                    if ( colourFmt == SpriteComponent.Palette && pixel == 0 )
                    {
                        sprite.data[i].a = 0x00;
                    }

                    pixel++;
                }
            }
        }

        return true;
    }

    public static byte LoadSpriteV1( ref SpriteCollection sprite, SpriteFile file, int filePos, SpriteType spriteType, bool load32bpp, byte avail8bpp )
    {
        // Check the requested colour depth
        if ( load32bpp )
        {
            return 0;
        }

        // Open the right file and go to the correct position
        file.SeekTo( filePos, Seek.Set );

        // Read the size and type
        int num = file.ReadWord();
        byte type = file.ReadByte();

        // Type 0xFF indicates either a colourmap or some other non-sprite info; we do not handle them there
        if ( type == 0xFF )
        {
            return 0;
        }

        ZoomLevel zoomLvl = ( spriteType != SpriteType.MapGen ) ? ZoomLevel.Normal : ZoomLevel.Min;

        sprite[(int)zoomLvl].height = file.ReadByte();
        sprite[(int)zoomLvl].width = file.ReadWord();
        sprite[(int)zoomLvl].xOffs = (short)file.ReadWord();
        sprite[(int)zoomLvl].yOffs = (short)file.ReadWord();
        sprite[(int)zoomLvl].colours = SpriteComponent.Palette;

        if ( sprite[(int)zoomLvl].width > short.MaxValue )
        {
            WarnCorruptSprite( file, filePos );
            return 0;
        }

        // 0x02 indicates it is a compressed sprite, so we can't rely on 'num' to be valid
        // In case it is uncompressed, the size is 'num' - 8 (header-size)
        num = ( type & 0x02 ) != 0 ? sprite[(int)zoomLvl].width * sprite[(int)zoomLvl].height : num - 8;

        if ( DecodeSingleSprite( ref sprite[(int)zoomLvl], file, filePos, spriteType, num, type, zoomLvl, SpriteComponent.Palette, 1 ) )
        {
            BitMath.SetBit( avail8bpp, (byte)zoomLvl );
            return avail8bpp;
        }

        return 0;
    }

    public byte LoadSpriteV2( ref SpriteCollection sprite, SpriteFile file, int filePos, SpriteType spriteType, bool load32bpp, byte controlFlags, byte avail8bpp, byte avail32bpp )
    {
        ZoomLevel[] zoomLvlMap = { ZoomLevel.Normal, ZoomLevel.In4x, ZoomLevel.In2x, ZoomLevel.Out2x, ZoomLevel.Out4x, ZoomLevel.Out8x };

        // Is the sprite not present / stripped in the GRF?
        if ( filePos == Size.Max )
        {
            return 0;
        }

        // Open the right file and go to the correct position
        file.SeekTo( filePos, Seek.Set );

        uint id = file.ReadDword();
        byte loadedSprites = 0;

        do
        {
            long num = file.ReadDword();
            int startPos = file.GetPos();
            byte type = file.ReadByte();

            // Type 0xFF indicates either a colourmap or some other non-sprite info; we do not handle them here
            if ( type == 0xFF )
            {
                return 0;
            }

            SpriteComponents colour = new SpriteComponents( type );

            // Mask out colour component information from type
            type &= ~SpriteComponents.Mask;

            byte zoom = file.ReadByte();

            bool isWantedColourDepth = colour != null && ( load32bpp ? colour != SpriteComponent.Palette : colour == SpriteComponent.Palette );
            bool isWantedZoomLvl;

            if ( spriteType != SpriteType.MapGen )
            {
                if ( zoom < zoomLvlMap.Length )
                {
                    if ( colour == SpriteComponent.Palette )
                    {
                        BitMath.SetBit( avail8bpp, (byte)zoomLvlMap[zoom] );
                    }

                    if ( colour != SpriteComponent.Palette )
                    {
                        BitMath.SetBit( avail32bpp, (byte)zoomLvlMap[zoom] );
                    }

                    isWantedZoomLvl = true;
                    ZoomLevel zoomMin = spriteType == SpriteType.Font ? ZoomLevel.Min : settingsClient.gui.spriteZoomMin;

                    if ( zoomMin >= ZoomLevel.In2x && BitMath.HasBit( controlFlags, load32bpp ? (byte)SpriteCacheCtrlFlags.AllowZoomMin2x32bpp : (byte)SpriteCacheCtrlFlags.AllowZoomMin2xPal ) && zoomLvlMap[zoom] < ZoomLevel.In2x )
                    {
                        isWantedZoomLvl = false;
                    }

                    if ( zoomMin >= ZoomLevel.Normal && BitMath.HasBit( controlFlags, load32bpp ? (byte)SpriteCacheCtrlFlags.AllowZoomMin1x32bpp : (byte)SpriteCacheCtrlFlags.AllowZoomMin1xPal ) && zoomLvlMap[zoom] < ZoomLevel.Normal )
                    {
                        isWantedZoomLvl = false;
                    }
                }
                else
                {
                    isWantedZoomLvl = false;
                }
            }
            else
            {
                isWantedZoomLvl = ( zoom == 0 );
            }

            if ( isWantedColourDepth && isWantedZoomLvl )
            {
                ZoomLevel zoomLvl = ( spriteType != SpriteType.MapGen ) ? zoomLvlMap[zoom] : ZoomLevel.Min;

                if ( BitMath.HasBit( loadedSprites, (byte)zoomLvl ) )
                {
                    // We already have this zoom level, skip sprite
                    throw new NotImplementedException( "Function \"Debug\" is not yet implemented!" );
                }

                sprite[(int)zoomLvl].height = file.ReadWord();
                sprite[(int)zoomLvl].width = file.ReadWord();
                sprite[(int)zoomLvl].xOffs = (short)file.ReadWord();
                sprite[(int)zoomLvl].yOffs = (short)file.ReadWord();

                if ( sprite[(int)zoomLvl].width > short.MaxValue || sprite[(int)zoomLvl].height > short.MaxValue )
                {
                    WarnCorruptSprite( file, filePos );
                    return 0;
                }

                // Convert colour components to pixel size
                int bpp = 0;

                if ( colour.Test( SpriteComponent.RGB ) )
                {
                    bpp += 3;
                }

                if ( colour.Test( SpriteComponent.Alpha ) )
                {
                    bpp++;
                }

                if ( colour.Test( SpriteComponent.Palette ) )
                {
                    bpp++;
                }

                sprite[(int)zoomLvl].colours = colour;

                // For chunked encoding we store the decompressed size in the file,
                // otherwise we can calculate it from the image dimensions
                uint decompSize = ( type & 0x08 ) != 0 ? file.ReadDword() : (uint)(sprite[(int)zoomLvl].width * sprite[(int)zoomLvl].height * bpp);

                bool valid = DecodeSingleSprite( sprite[(int)zoomLvl], file, filePos, spriteType, type, zoomLvl, colour, 2 );

                if ( file.GetPos() != startPos + num )
                {
                    WarnCorruptSprite( file, filePos );
                    return 0;
                }

                if ( valid )
                {
                    BitMath.SetBit( loadedSprites, (byte)zoomLvl );
                }
            }
            else
            {
                // Not the wanted zoom level or colour depth, continue searching
                file.SkipBytes( num - 2 );
            }
        } while ( file.ReadDword() == id );

        return loadedSprites;
    }
}

/// <summary>
/// Interface for something that can allocate memory for a sprite.
/// </summary>
public class SpriteAllocator
{
    /// <summary>
    /// Allocate memory for a sprite.
    /// </summary>
    /// <typeparam name="T">Type to return memory as.</typeparam>
    /// <param name="size">Size of memory to allocate in bytes.</param>
    /// <returns>Pointer to allocated memory.</returns>
    public T Allocate<T>( int size )
    {
        return (T)AllocatePtr( size );
    }

    /// <summary>
    /// Allocate memory for a sprite.
    /// </summary>
    /// <param name="size">Size of memory to allocate.</param>
    /// <returns>Pointer to allocated memory.</returns>
    protected virtual object AllocatePtr( int size )
    {
        return null;
    }
}

/// <summary>
/// Interface for something that can encode a sprite.
/// </summary>
public class SpriteEncoder
{
    public virtual bool Is32BppSupported()
    {
        return false;
    }

    public virtual Sprite Encode( SpriteCollection sprite, SpriteAllocator allocator )
    {
        return default;
    }

    public virtual uint GetSpriteAlignment()
    {
        return 0;
    }
}