using System;

namespace OpenTTD.SpriteLoader;

public class SpriteLoaderMakeIndexed : SpriteLoader
{
    public SpriteLoader baseLoader;

    public SpriteLoaderMakeIndexed( SpriteLoader baseLoader )
    {
        this.baseLoader = baseLoader;
    }

    public override byte LoadSprite( ref SpriteCollection sprite, SpriteFile file, int filePos, SpriteType spriteType, bool load32bpp, byte controlFlags, byte avail8bpp, byte avail32bpp )
    {
        byte avail = baseLoader.LoadSprite(ref sprite, file, filePos, spriteType, load32bpp, controlFlags, avail8bpp, avail32bpp );

        for ( ZoomLevel zoom = ZoomLevel.Begin; zoom != ZoomLevel.End; zoom++ )
        {
            if ( BitMath.HasBit( avail, zoom ) )
            {
                Convert32bppTo8bpp( sprite[(int)zoom] );
            }
        }

        return avail;
    }
}