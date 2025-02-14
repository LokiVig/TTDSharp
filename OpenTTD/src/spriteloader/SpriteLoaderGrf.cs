namespace OpenTTD.SpriteLoader;

public class SpriteLoaderGrf : SpriteLoader
{
    public byte containerVer;

    public SpriteLoaderGrf( byte containerVer )
    {
        this.containerVer = containerVer;
    }

    public override byte LoadSprite( ref SpriteCollection sprite, SpriteFile file, int filePos, SpriteType spriteType, bool load32bpp, byte controlFlags, byte avail8bpp, byte avail32bpp )
    {
        if ( containerVer >= 2 )
        {
            return LoadSpriteV2( ref sprite, file, filePos, spriteType, load32bpp, controlFlags, avail8bpp, avail32bpp );
        }
        else
        {
            return LoadSpriteV1( ref sprite, file, filePos, spriteType, load32bpp, avail8bpp );
        }
    }
}