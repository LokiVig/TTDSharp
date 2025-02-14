using System.Runtime.InteropServices;

using OpenTTD.SpriteLoader;

namespace OpenTTD.Blitter;

public class BlitterNull : Blitter
{
    public override byte GetScreenDepth()
    {
        return 0;
    }

    public override void Draw( BlitterParams bp, BlitterMode mode, ZoomLevel zoom )
    {
        return;
    }

    public override void DrawColourMappingRect( object dst, int width, int height, PaletteID pal )
    {
        return;
    }

    public override Sprite Encode( SpriteLoader.SpriteLoader.SpriteCollection sprite, SpriteAllocator allocator )
    {
        Sprite destSprite;
        destSprite = allocator.Allocate<Sprite>( Marshal.SizeOf( destSprite ) );

        destSprite.height = sprite[ZoomLevel.Min].height;
        destSprite.width = sprite[ZoomLevel.Min].width;
        destSprite.xOffs = sprite[ZoomLevel.Min].xOffs;
        destSprite.yOffs = sprite[ZoomLevel.Min].yOffs;

        return destSprite;
    }

    public override object MoveTo( object video, int x, int y )
    {
        return null;
    }

    public override void SetPixel( object video, int x, int y, byte colour )
    {
        return;
    }

    public override void DrawRect( object video, int width, int height, byte colour )
    {
        return;
    }

    public override void DrawLine( object video, int x1, int y1, int x2, int y2, int screenWidth, int screenHeight, byte colour, int width, int dash = 0 )
    {
        return;
    }

    public override void CopyFromBuffer( object video, object src, int width, int height )
    {
        return;
    }

    public override void CopyToBuffer( object video, object dst, int width, int height )
    {
        return;
    }

    public override void CopyImageToBuffer( object video, object dst, int width, int height, int dstPitch )
    {
        return;
    }

    public override void ScrollBuffer( object video, int left, int top, int width, int height, int scrollX, int scrollY )
    {
        return;
    }

    public override int BufferSize( uint width, uint height )
    {
        return 0;
    }

    public override void PaletteAnimate( Palette palette )
    {
        return;
    }

    public override PaletteAnimation UsePaletteAnimation()
    {
        return PaletteAnimation.None;
    }

    public override string GetName()
    {
        return "null";
    }
}

public class FBlitterNull : BlitterFactory
{
    protected FBlitterNull() : base("null", "Null Blitter (does nothing)") { }

    public override Blitter CreateInstance()
    {
        return new BlitterNull();
    }
}