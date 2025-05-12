using System;

namespace OpenTTD.Core;

/// <summary>
/// Coordinates of a point in 2D.
/// </summary>
public struct Point
{
    public int x;
    public int y;

    public Point()
    {
        x = 0;
        y = 0;
    }

    public Point( int x, int y )
    {
        this.x = x;
        this.y = y;
    }
}

/// <summary>
/// Dimensions (a width and a height) of a rectangle in 2D.
/// </summary>
public struct Dimension
{
    public uint width;
    public uint height;

    public Dimension()
    {
        width = 0;
        height = 0;
    }

    public Dimension( uint width, uint height )
    {
        this.width = width;
        this.height = height;
    }

    /// <summary>
    /// Compute bounding box of both dimensions.
    /// </summary>
    /// <param name="d1">First dimension.</param>
    /// <param name="d2">Second dimension.</param>
    /// <returns>The bounding box of both dimensions, the smallest dimension that surrounds both arguments.</returns>
    public static Dimension MaxDim( Dimension d1, Dimension d2 )
    {
        Dimension d = new Dimension();
        d.width = Math.Max( d1.width, d2.width );
        d.height = Math.Max( d1.height, d2.height );
        return d;
    }

    public static bool operator <( Dimension lhs, Dimension rhs )
    {
        int x = (int)(lhs.width - rhs.width);

        if ( x != 0 )
        {
            return x < 0;
        }

        return lhs.height < rhs.height;
    }

    public static bool operator >( Dimension lhs, Dimension rhs )
    {
        int x = (int)( lhs.width - rhs.width );

        if ( x != 0 )
        {
            return x > 0;
        }

        return lhs.height > rhs.height;
    }

    public static bool operator ==( Dimension lhs, Dimension rhs )
    {
        return lhs.width == rhs.width && lhs.height == rhs.height;
    }

    public static bool operator !=( Dimension lhs, Dimension rhs )
    {
        return lhs.width != rhs.width && lhs.height != rhs.height;
    }
}

/// <summary>
/// Padding dimensions to apply to each side of a <see cref="Rect"/>.
/// </summary>
public struct RectPadding
{
    public byte left;
    public byte top;
    public byte right;
    public byte bottom;

    public static readonly RectPadding Zero;

    /// <summary>
    /// Get total horizontal padding of <see cref="RectPadding"/>.
    /// </summary>
    /// <returns>Total horizontal padding.</returns>
    public uint Horizontal()
    {
        return (uint)(left + right);
    }

    /// <summary>
    /// Get total vertical padding of <see cref="RectPadding"/>.
    /// </summary>
    /// <returns>Total vertical padding.</returns>
    public uint Vertical()
    {
        return (uint)( top + bottom );
    }
}

/// <summary>
/// Specification of a rectangle with absolute coordinates of all edges.
/// </summary>
public struct Rect
{
    public int left;
    public int top;
    public int right;
    public int bottom;

    public Rect( int left, int top, int right, int bottom )
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }

    /// <summary>
    /// Get width of <see cref="Rect"/>.
    /// </summary>
    /// <returns>Width of <see cref="Rect"/>.</returns>
    public int Width()
    {
        return right - left + 1;
    }

    /// <summary>
    /// Get height of <see cref="Rect"/>.
    /// </summary>
    /// <returns>Height of <see cref="Rect"/>.</returns>
    public int Height()
    {
        return bottom - top + 1;
    }

    /// <summary>
    /// Copy and shrink <see cref="Rect"/> by <paramref name="s"/> pixels.
    /// </summary>
    /// <param name="s">Number of pixels to remove from each side of <see cref="Rect"/>.</param>
    /// <returns>The new, smaller <see cref="Rect"/>.</returns>
    public Rect Shrink( int s )
    {
        return new Rect( left + s, top + s, right - s, bottom - s );
    }

    /// <summary>
    /// Copy and shrink <see cref="Rect"/> by <paramref name="h"/> horizontal and <paramref name="v"/> vertical pixels.
    /// </summary>
    /// <param name="h">Number of pixels to remove from left and right sides.</param>
    /// <param name="v">Number of pixels to remove from top and bottom sides.</param>
    /// <returns>The new, smaller <see cref="Rect"/>.</returns>
    public Rect Shrink( int h, int v )
    {
        return new Rect( left + h, top + v, right - h, bottom - v );
    }

    /// <summary>
    /// Copy and shrink <see cref="Rect"/> by pixels.
    /// </summary>
    /// <param name="left">Number of pixels to remove from left side.</param>
    /// <param name="top">Number of pixels to remove from top side.</param>
    /// <param name="right">Number of pixels to remove from right side.</param>
    /// <param name="bottom">Number of pixels to remove from bottom side.</param>
    /// <returns>The new, smaller <see cref="Rect"/>.</returns>
    public Rect Shrink( int left, int top, int right, int bottom )
    {
        return new Rect( this.left + left, this.top + top, this.right - right, this.bottom - bottom );
    }

    /// <summary>
    /// Copy and shrink <see cref="Rect"/> by a <see cref="RectPadding"/>.
    /// </summary>
    /// <param name="other"><see cref="RectPadding"/> to remove from each side of <see cref="Rect"/>.</param>
    /// <returns>The new, smaller <see cref="Rect"/>.</returns>
    public Rect Shrink( RectPadding other )
    {
        return new Rect( left + other.left, top + other.top, right - other.right, bottom - other.bottom );
    }

    /// <summary>
    /// Copy and shrink <see cref="Rect"/> by a different horizontal and vertical <see cref="RectPadding"/>.
    /// </summary>
    /// <param name="horz"><see cref="RectPadding"/> to remove from left and right of <see cref="Rect"/>.</param>
    /// <param name="vert"><see cref="RectPadding"/> to remove from top and bottom of <see cref="Rect"/>.</param>
    /// <returns>The new, smaller <see cref="Rect"/>.</returns>
    public Rect Shrink( RectPadding horz, RectPadding vert )
    {
        return new Rect( left + horz.left, top + vert.top, right - horz.right, bottom - vert.bottom );
    }

    /// <summary>
    /// Copy and expand <see cref="Rect"/> by <paramref name="s"/> pixels.
    /// </summary>
    /// <param name="s">Number of pixels to add to each side of <see cref="Rect"/>.</param>
    /// <returns>The new, larger <see cref="Rect"/>.</returns>
    public Rect Expand( int s )
    {
        return Shrink( -s );
    }

    /// <summary>
    /// Copy and expand <see cref="Rect"/> by a <see cref="RectPadding"/>.
    /// </summary>
    /// <param name="other"><see cref="RectPadding"/> to add to each side of <see cref="Rect"/>.</param>
    /// <returns>The new, larger <see cref="Rect"/>.</returns>
    public Rect Expand( RectPadding other )
    {
        return new Rect( left - other.left, top - other.top, right + other.right, bottom + other.bottom );
    }

    /// <summary>
    /// Copy and translate <see cref="Rect"/> by <paramref name="x"/>, <paramref name="y"/> pixels.
    /// </summary>
    /// <param name="x">Number of pixels to move horizontally.</param>
    /// <param name="y">Number of pixels to move vertically.</param>
    /// <returns>The new, translated <see cref="Rect"/>.</returns>
    public Rect Translate( int x, int y )
    {
        return new Rect( left + x, top + y, right + x, bottom + y );
    }

    /// <summary>
    /// Copy <see cref="Rect"/> and set its width.
    /// </summary>
    /// <param name="width">Width in pixels for new <see cref="Rect"/>.</param>
    /// <param name="end">If set, set <paramref name="width"/> at end of <see cref="Rect"/>, i.e. on the right.</param>
    /// <returns>The new, resized <see cref="Rect"/>.</returns>
    public Rect WithWidth( int width, bool end )
    {
        return end
            ? new Rect( right - width + 1, top, right,            bottom )
            : new Rect( left,              top, left + width - 1, bottom );
    }

    /// <summary>
    /// Copy <see cref="Rect"/> and indent it from its position.
    /// </summary>
    /// <param name="indent">Offset in pixels for new <see cref="Rect"/>.</param>
    /// <param name="end">If set, set indent at end of <see cref="Rect"/>, i.e. on the right.</param>
    /// <returns>The new, resized <see cref="Rect"/>.</returns>
    public Rect Indent( int indent, bool end )
    {
        return end
            ? new Rect( left,          top, right - indent, bottom )
            : new Rect( left + indent, top, right,          bottom );
    }

    /// <summary>
    /// Copy <see cref="Rect"/> and set its height.
    /// </summary>
    /// <param name="height">Height in pixels for new <see cref="Rect"/>.</param>
    /// <param name="end">If set, set height at end of <see cref="Rect"/>, i.e. at the bottom.</param>
    /// <returns>The new, resized <see cref="Rect"/>.</returns>
    public Rect WithHeight( int height, bool end )
    {
        return end
            ? new Rect( left, bottom - height + 1, right,           bottom )
            : new Rect( left, top,                 right, top + height - 1 );
    }

    /// <summary>
    /// Test if a <see cref="Point"/> is inside this <see cref="Rect"/>.
    /// </summary>
    /// <param name="pt">The <see cref="Point"/> to test.</param>
    /// <returns><see langword="true"/> if the point falls inside the <see cref="Rect"/>.</returns>
    public bool Contains( Point pt )
    {
        // This is a local version of IsInsideMM, to avoid íncluding MathFunc everywhere // Not really necessary now in C#
        return (uint)( pt.x - left ) < (uint)( right - left ) && (uint)( pt.y - top ) < (uint)( bottom - top );
    }

    /// <summary>
    /// Check if a rectangle is empty.
    /// </summary>
    /// <param name="rect">Rectangle to check.</param>
    /// <returns><see langword="true"/> if and only if the rectangle doesn't define space.</returns>
    public static bool IsEmptyRect( Rect rect )
    {
        return ( rect.left | rect.top | rect.right | rect.bottom ) == 0;
    }

    /// <summary>
    /// Compute the bounding rectangle around two rectangles.
    /// </summary>
    /// <param name="r1">First rectangle.</param>
    /// <param name="r2">Second rectangle.</param>
    /// <returns>The bounding rectangle, the smallest rectangle that contains both arguments.</returns>
    public static Rect BoundingRect( Rect r1, Rect r2 )
    {
        // If either the first or the second rect is empty, return the other
        if ( IsEmptyRect( r1 ) )
        {
            return r2;
        }

        if ( IsEmptyRect( r2 ) )
        {
            return r1;
        }

        Rect r = new Rect();

        r.top = Math.Min( r1.top, r2.top );
        r.bottom = Math.Max( r1.bottom, r2.bottom );
        r.left = Math.Min( r1.left, r2.left );
        r.right = Math.Max( r1.right, r2.right );

        return r;
    }
}

/// <summary>
/// Specification of a rectangle with an absolute top-left coordinate and a<br/>
/// (relative) width / height.
/// </summary>
public struct PointDimension
{
    public int x;
    public int y;
    public int width;
    public int height;
}