using System;

using OpenTTD.SpriteLoader;

namespace OpenTTD.Blitter;

/// <summary>
/// The modes of blitting we can do.
/// </summary>
public enum BlitterMode : byte
{
    Normal,
    ColourRemap,
    Transparent,
    TransparentRemap,
    CrashRemap,
    BlackRemap
}

/// <summary>
/// How all blitters should look like.<br/>
/// Extend this class to make your own.
/// </summary>
public class Blitter : SpriteEncoder
{
    /// <summary>
    /// Parameters related to blitting.
    /// </summary>
    public struct BlitterParams
    {
        public object sprite;
        public byte[] remap;

        public int skipLeft;
        public int skipTop;
        public int width;
        public int height;
        public int spriteWidth;
        public int spriteHeight;
        public int left;
        public int top;

        public object dst;
        public int pitch;
    }

    /// <summary>
    /// Types of palette animation.
    /// </summary>
    public enum PaletteAnimation : byte
    {
        None,
        VideoBackend,
        Blitter
    }

    /// <summary>
    /// Get the screen depth this blitter works for.<br/>
    /// <para>
    ///     This is either: 8, 16, 24 or 32.
    /// </para>
    /// </summary>
    public virtual byte GetScreenDepth()
    {
        return 0;
    }

    public override bool Is32BppSupported()
    {
        return GetScreenDepth() > 8;
    }

    /// <summary>
    /// Draw an image to the screen, given an amount of params defined above.
    /// </summary>
    public virtual void Draw( BlitterParams bp, BlitterMode mode, ZoomLevel zoom )
    {

    }

    /// <summary>
    /// Draw a colourtable to the screen.<br/>
    /// This is: the colour of the screen is read and is looked-up in the palette<br/>
    /// to match a new colour, which then is put on the screen again.
    /// </summary>
    /// <param name="dst">The destination object (video-buffer).</param>
    /// <param name="width">The width of the buffer.</param>
    /// <param name="height">The height of the buffer.</param>
    /// <param name="pal">The palette to use.</param>
    public virtual void DrawColourMappingRect( object dst, int width, int height, PaletteID pal )
    {

    }

    /// <summary>
    /// Move the destination pointer the requested amount of x and y, keeping in mind<br/>
    /// any pitch and bpp of the renderer.
    /// </summary>
    /// <param name="video">The destination object (video-buffer) to scroll.</param>
    /// <param name="x">How much you want to scroll to the right.</param>
    /// <param name="y">How much you want to scroll to the bottom.</param>
    /// <returns>A new destination object moved to the requested place.</returns>
    public virtual object MoveTo( object video, int x, int y )
    {
        return null;
    }

    /// <summary>
    /// Draw a pixel with a given colour on the video-buffer.
    /// </summary>
    /// <param name="video">The destination object (video-buffer).</param>
    /// <param name="x">The X position within video-buffer.</param>
    /// <param name="y">The Y position within video-buffer.</param>
    /// <param name="colour">A 8bpp mapping colour.</param>
    public virtual void SetPixel( object video, int x, int y, byte colour )
    {

    }

    /// <summary>
    /// Make a single horizontal line in a single colour on the video-buffer.
    /// </summary>
    /// <param name="video">The destination object (video-buffer).</param>
    /// <param name="width">The length of the line.</param>
    /// <param name="height">The height of the line.</param>
    /// <param name="colour">A 8bpp mapping colour.</param>
    public virtual void DrawRect( object video, int width, int height, byte colour )
    {

    }

    /// <summary>
    /// Draw a line with a given colour.
    /// </summary>
    /// <param name="video">The destination object (video-buffer).</param>
    /// <param name="x1">The X coordinate from where the line starts.</param>
    /// <param name="y1">The Y coordinate from where the line starts.</param>
    /// <param name="x2">The X coordinate to where the line goes.</param>
    /// <param name="y2">The Y coordinate to where the line goes.</param>
    /// <param name="screenWidth">The width of the screen you're drawing in (to avoid buffer-overflows).</param>
    /// <param name="screenHeight">The height of the screen you're drawing in (to avoid buffer-overflows).</param>
    /// <param name="colour">A 8bpp mapping colour.</param>
    /// <param name="width">Line width.</param>
    /// <param name="dash">Length of dashes for dashed lines. 0 means solid line.</param>
    public virtual void DrawLine( object video, int x1, int y1, int x2, int y2, int screenWidth, int screenHeight, byte colour, int width, int dash = 0 )
    {

    }

    /// <summary>
    /// Copy from a buffer to the screen.<br/>
    /// Note: You can not do anything with the content of this buffer, as the blitter can store non-pixel data in it too!
    /// </summary>
    /// <param name="video">The destination object (video-buffer).</param>
    /// <param name="src">The buffer from which the data will be read.</param>
    /// <param name="width">The width of the buffer.</param>
    /// <param name="height">The height of the buffer.</param>
    public virtual void CopyFromBuffer( object video, object src, int width, int height )
    {

    }

    /// <summary>
    /// Copy from the screen to a buffer.<br/>
    /// Note: You can not do anything with the content of this buffer, as the blitter can store non-pixel data in it too!
    /// </summary>
    /// <param name="video">The destination object (video-buffer).</param>
    /// <param name="dst">The buffer in which the data will be stored.</param>
    /// <param name="width">The width of the buffer.</param>
    /// <param name="height">The height of the buffer.</param>
    public virtual void CopyToBuffer( object video, object dst, int width, int height )
    {

    }

    /// <summary>
    /// Copy from the screen to a buffer in a palette format for 8bpp and RGBA format for 32bpp.
    /// </summary>
    /// <param name="video">The destination object (video-buffer).</param>
    /// <param name="dst">The buffer in which the data will be stored.</param>
    /// <param name="width">The width of the buffer.</param>
    /// <param name="height">The height of the buffer.</param>
    /// <param name="dstPitch">The pitch (byte per line) of the destination buffer.</param>
    public virtual void CopyImageToBuffer( object video, object dst, int width, int height, int dstPitch )
    {

    }

    /// <summary>
    /// Scroll the video-buffer some '<c>x</c>' and '<c>y</c>' value.
    /// </summary>
    /// <param name="video">The buffer to scroll into.</param>
    /// <param name="left">The left value of the screen to scroll.</param>
    /// <param name="top">The top value of the screen to scroll.</param>
    /// <param name="width">The width of the screen to scroll.</param>
    /// <param name="height">The height of the screen to scroll.</param>
    /// <param name="scrollX">How much to scroll in X.</param>
    /// <param name="scrollY">How much to scroll in Y.</param>
    public virtual void ScrollBuffer( object video, int left, int top, int width, int height, int scrollX, int scrollY )
    {

    }

    /// <summary>
    /// Calculate how much memory there is needed for an image of this size in the video-buffer.
    /// </summary>
    /// <param name="width">The width of the buffer-to-be.</param>
    /// <param name="height">The height of the buffer-to-be.</param>
    /// <returns>The size needed for the buffer.</returns>
    public virtual int BufferSize( uint width, uint height )
    {
        return 0;
    }

    /// <summary>
    /// Called when the 8bpp palette is changed; you should redraw all pixels on the screen that<br/>
    /// are equal to the 8bpp palette indexes '<c>firstDirty</c>' to '<c>firstDirty + countDirty</c>'.
    /// </summary>
    /// <param name="palette">The new palette.</param>
    public virtual void PaletteAnimate( Palette palette )
    {

    }

    /// <summary>
    /// Check if the blitter uses palette animation at all.
    /// </summary>
    /// <returns><see langword="true"/> if it uses palette animation.</returns>
    public virtual PaletteAnimation UsePaletteAnimation()
    {
        return PaletteAnimation.None;
    }

    /// <summary>
    /// Does this blitter require a separate animation buffer from the video backend?
    /// </summary>
    public virtual bool NeedsAnimationBuffer()
    {
        return false;
    }

    /// <summary>
    /// Get the name of the blitter, the same as the Factory-instance returns.
    /// </summary>
    public virtual string GetName()
    {
        return string.Empty;
    }

    /// <summary>
    /// Post resize event.
    /// </summary>
    public event Action PostResize;

    public void DrawLineGeneric( int x1, int y1, int x2, int y2, int screenWidth, int screenHeight, int width, int dash, Action<int, int> setPixel )
    {
        int dy;
        int dx;
        int stepX;
        int stepY;

        dy = ( y2 - y1 ) * 2;

        if ( dy < 0 )
        {
            dy = -dy;
            stepY = -1;
        }
        else
        {
            stepY = 1;
        }

        dx = ( x2 - x1 ) * 2;

        if ( dx < 0 )
        {
            dx = -dx;
            stepX = -1;
        }
        else
        {
            stepX = 1;
        }

        if ( dx == 0 && dy == 0 )
        {
            // The algorithm below cannot handle this special case; make it work at least for line width 1
            if ( x1 >= 0 && x1 < screenWidth && y1 >= 0 && y1 < screenHeight )
            {
                setPixel( x1, y1 );
                return;
            }
        }

        int fracDiff = width * Math.Max( dx, dy );

        if ( width > 1 )
        {
            // Compute fracDiff = width * Math.Sqrt(dx*dx + dy*dy)
            // Start interval:
            //  Math.Max(dx, dy) <= Math.Sqrt(dx*dx + dy*dy) <= Math.Sqrt(2) * Math.Max(dx, dy) <= 3/2 * Math.Max(dx, dy)
            long fracSq = width * width * ( dx * dx + dy * dy );
            int fracMax = 3 * fracDiff / 2;

            while ( fracDiff < fracMax )
            {
                int fracTest = ( fracDiff + fracMax ) / 2;

                if ( fracTest * fracTest < fracSq )
                {
                    fracDiff = fracTest + 1;
                }
                else
                {
                    fracMax = fracTest - 1;
                }
            }
        }

        int gap = dash;

        if ( dash == 0 )
        {
            dash = 1;
        }

        int dashCount = 0;

        if ( dx > dy )
        {
            if ( stepX < 0 )
            {
                (x1, x2) = (x2, x1);
                (y1, y2) = (y2, y1);
                stepY = -stepY;
            }

            if ( x2 < 0 || x1 >= screenWidth )
            {
                return;
            }

            int yLow = y1;
            int yHigh = y1;
            int fracLow = dy - fracDiff / 2;
            int fracHigh = dy + fracDiff / 2;

            while ( fracLow < -( dx / 2 ) )
            {
                fracLow += dx;
                yLow -= stepY;
            }

            while ( fracHigh >= dx / 2 )
            {
                fracHigh -= dx;
                yHigh += stepY;
            }

            if ( x1 < 0 )
            {
                dashCount = ( -x1 ) % ( dash + gap );

                Func<long, int, int> adjustFrac = ( long frac, int yBound ) =>
                {
                    frac -= dy * x1;

                    if ( frac >= 0 )
                    {
                        int quotient = (int)frac / dx;
                        int remainder = (int)frac % dx;

                        yBound += ( 1 + quotient ) * stepY;
                        frac = remainder - dx;
                    }

                    return (int)frac;
                };

                fracLow = adjustFrac( fracLow, yLow );
                fracHigh = adjustFrac( fracHigh, yHigh );
                x1 = 0;
            }

            x2++;

            if ( x2 > screenWidth )
            {
                x2 = screenWidth;
            }

            while ( x1 != x2 )
            {
                if ( dashCount < dash )
                {
                    for ( int y = yLow; y != yHigh; y += stepY )
                    {
                        if ( y >= 0 && y < screenHeight )
                        {
                            setPixel( x1, y );
                        }
                    }
                }

                if ( fracLow >= 0 )
                {
                    yLow += stepY;
                    fracLow -= dx;
                }

                if ( fracHigh >= 0 )
                {
                    yHigh += stepY;
                    fracHigh -= dx;
                }

                x1++;
                fracLow += dy;
                fracHigh += dy;

                if ( ++dashCount >= dash + gap )
                {
                    dashCount = 0;
                }
            }
        }
        else
        {
            if ( stepY < 0 )
            {
                (x1, x2) = (x2, x1);
                (y1, y2) = (y2, y1);
                stepX = -stepX;
            }

            if ( y2 < 0 || y1 >= screenHeight )
            {
                return;
            }

            int xLow = x1;
            int xHigh = x1;
            int fracLow = dx - fracDiff / 2;
            int fracHigh = dx + fracDiff / 2;

            while ( fracLow < -( dy / 2 ) )
            {
                fracLow += dy;
                xLow -= stepX;
            }

            while ( fracHigh >= dy / 2 )
            {
                fracHigh -= dy;
                xHigh += stepX;
            }

            if ( y1 < 0 )
            {
                dashCount = ( -y1 ) % ( dash + gap );

                Func<long, int, int> adjustFrac = ( long frac, int xBound ) =>
                {
                    frac -= dx * y1;

                    if ( frac >= 0 )
                    {
                        int quotient = (int)frac / dy;
                        int remainder = (int)frac % dy;
                        xBound += ( 1 + quotient ) * stepX;
                        frac = remainder - dy;
                    }

                    return (int)frac;
                };

                fracLow = adjustFrac( fracLow, xLow );
                fracHigh = adjustFrac( fracHigh, xHigh );
                y1 = 0;
            }

            y2++;

            if ( y2 > screenHeight )
            {
                y2 = screenHeight;
            }

            while ( y1 != y2 )
            {
                if ( dashCount < dash )
                {
                    for ( int x = xLow; x != xHigh; x += stepX )
                    {
                        if ( x >= 0 && x < screenWidth )
                        {
                            setPixel( x, y1 );
                        }
                    }
                }

                if ( fracLow >= 0 )
                {
                    xLow += stepX;
                    fracLow -= dy;
                }

                if ( fracHigh >= 0 )
                {
                    xHigh += stepX;
                    fracHigh -= dy;
                }

                y1++;
                fracLow += dx;
                fracHigh += dx;

                if ( ++dashCount >= dash + gap )
                {
                    dashCount = 0;
                }
            }
        }
    }
}