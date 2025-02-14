global using SpriteID = uint;
global using PaletteID = uint;
global using CursorID = uint;

using System.Collections.Generic;

namespace OpenTTD;

/// <summary>
/// Combination of a palette sprite and a 'real' sprite.
/// </summary>
public struct PalSpriteID
{
    public SpriteID sprite; // The 'real' sprite
    public PaletteID pal; // The palette (use "None" if not needed)
}

public enum WindowsKeyCodes : ushort
{
    Shift = 0x8000,
    Ctrl = 0x4000,
    Alt = 0x2000,
    Meta = 0x1000,

    GlobalHotkey = 0x0800, // Fake keycode bit to indicate global hotkeys

    SpecialKeys = Shift | Ctrl | Alt | Meta | GlobalHotkey,

    // Special ones
    None = 0,
    Esc = 1,
    Backspace = 2,
    Insert = 3,
    Delete = 4,

    PageUp = 5,
    PageDown = 6,
    End = 7,
    Home = 8,

    // Arrow keys
    Left = 9,
    Up = 10,
    Right = 11,
    Down = 12,

    // Return & tab
    Return = 13,
    Tab = 14,

    // Space
    Space = 32,

    // Function keys
    F1 = 33,
    F2 = 34,
    F3 = 35,
    F4 = 36,
    F5 = 37,
    F6 = 38,
    F7 = 39,
    F8 = 40,
    F9 = 41,
    F10 = 42,
    F11 = 43,
    F12 = 44,

    // Backqoute is the key left of "1"
    // We only store this key here, no matter what character is really mapped to it
    // on a particular keyboard (US keyboard: ` and ~; German keyboard: ^ and °)
    Backqoute = 45,
    Pause = 46,

    // 0-9 are mapped to 48-57
    // A-Z are mapped to 65-90
    // a-z are mapped to 97-122

    // Numerical keyboard
    NumDiv = 138,
    NumMul = 139,
    NumMinus = 140,
    NumPlus = 141,
    NumEnter = 142,
    NumDecimal = 143,

    // Other keys
    Slash = 144, // / Forward slash
    Semicolon = 145, // ; Semicolon
    Equals = 146, // = Equals
    LBracket = 147, // [ Left square bracket
    Backslash = 148, // \ Backslash
    RBracket = 149, // ] Right square bracket
    SingleQuote = 150, // ' Single quote
    Comma = 151, // , Comma
    Period = 152, // . Period
    Minus = 153, // - Minus
}

/// <summary>
/// A single sprite of a list of animated cursors.
/// </summary>
public struct AnimCursor
{
    public const CursorID LAST = CursorID.MaxValue;
    public CursorID sprite; // Must be set to LAST_ANIM when it is the last sprite of the loop
    public byte displayTime; // Amount of ticks this sprite will be shown
}

public struct CursorSprite
{
    public PalSpriteID image; // Image
    public Point pos; // Relative position

    public CursorSprite( SpriteID spr, PaletteID pal, int x, int y )
    {
        image.sprite = spr;
        image.pal = pal;

        pos.x = x;
        pos.y = y;
    }
}

/// <summary>
/// Collection of variables for cursor-display and -animation.
/// </summary>
public struct CursorVars
{
    public Point pos; // Logical mouse position
    public Point delta; // Relative mouse movement in this tick
    public int wheel; // Mouse wheel movement
    public bool fixAt; // Mouse is moving, but cursor is not (used for scrolling)

    // 2D wheel scrolling for moving around the map
    public bool wheelMoved;
    public float vWheel;
    public float hWheel;

    // Mouse appearance
    public List<CursorSprite> sprites; // Sprites comprising cursor
    public Point totalOffs, totalSize; // Union of sprite properties

    public Point drawPos, drawSize; // Position and size bounding-box for drawing

    public AnimCursor[] animateList; // In case of animated cursor, list of frames
    public AnimCursor animateCur; // In case of animated cursor, current frame
    public uint animateTimeout; // In case of animated cursor, number of ticks to show the current cursor

    public bool visible; // Cursor is visible
    public bool dirty; // The rect occupied by the mouse is dirty (redraw)
    public bool inWindow; // Mouse inside this window, determines drawing logic

    // Drag data
    public bool vehChain; // Vehicle chain is dragged

    public void UpdateCursorPositionRelative( int deltaX, int deltaY )
    {

    }

    public bool UpdateCursorPosition( int x, int y )
    {
        return false;
    }
}

/// <summary>
/// Data about how and where to blit pixels.
/// </summary>
public struct DrawPixelInfo
{
    public object dstPtr;
    public int left, top, width, height;
    public int pitch;
    public ZoomLevel zoom;
}

/// <summary>
/// Packed colour union to access the alpha, red, green and blue channels from a 32 bit number for Emscripten build.
/// </summary>
public struct ColourRGBA
{
    public uint data; // Conversion of the channel information to a 32 bit number
    public byte r, g, b, a; // Colour channels as used in browsers


    /// <summary>
    /// Create a new colour.
    /// </summary>
    /// <param name="r">The channel for the red colour.</param>
    /// <param name="g">The channel for the green colour.</param>
    /// <param name="b">The channel for the blue colour.</param>
    /// <param name="a">The channel for the alpha/transparency.</param>
    public ColourRGBA( byte r, byte g, byte b, byte a = 0xFF )
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    /// <summary>
    /// Create a new colour.
    /// </summary>
    /// <param name="data">The colour in the correct packed format.</param>
    public ColourRGBA( uint data = 0 )
    {
        this.data = data;
    }
}

/// <summary>
/// Packed colour struct to access the alpha, red, green and blue channels from a 32 bit number for big-endian systems.
/// </summary>
public struct ColourARGB
{
    public uint data; // Conversion of the channel information to a 32 bit number
    public byte a, r, g, b; // Colour channels in BE order

    /// <summary>
    /// Create a new colour.
    /// </summary>
    /// <param name="r">The channel for red color.</param>
    /// <param name="g">The channel for green color.</param>
    /// <param name="b">The channel for blue color.</param>
    /// <param name="a">The channel for alpha/transparency.</param>
    public ColourARGB( byte r, byte g, byte b, byte a = 0xFF )
    {
        this.a = a;
        this.r = r;
        this.g = g;
        this.b = b;
    }

    /// <summary>
    /// Create a new colour.
    /// </summary>
    /// <param name="data">The colour in the correct packed format.</param>
    public ColourARGB( uint data = 0 )
    {
        this.data = data;
    }
}

public struct ColourBGRA
{
    public uint data; // Conversion of the channel information to a 32 bit number
    public byte b, g, r, a; // Colour channels in LE order

    /// <summary>
    /// Create a new colour.
    /// </summary>
    /// <param name="r">The channel for the red color.</param>
    /// <param name="g">The channel for the green color.</param>
    /// <param name="b">The channel for the blue color.</param>
    /// <param name="a">The channel for the alpha/transparency.</param>
    public ColourBGRA( byte r, byte g, byte b, byte a = 0xFF )
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    /// <summary>
    /// Create a new colour.
    /// </summary>
    /// <param name="data">The colour in the correct packed format.</param>
    public ColourBGRA( uint data = 0 )
    {
        this.data = data;
    }
}

public enum FontSize : byte
{
    Normal, // Index of the normal font in the font tables
    Small, // Index of the small font in the font tables
    Large, // Index of the large font in the font tables
    Mono, // Index of the monospaced font in the font tables
    End,

    Begin = Normal // First font
}

/// <summary>
/// Used to only draw a part of the sprite.<br/>
/// Draw the subsprite in the rect (<c>spriteXOffset + left</c>, <c>spriteYOffset + top</c>) to (<c>spriteXOffset + right</c>, <c>spriteYOffset + bottom</c>).<br/>
/// Both corners are included in the drawing area.
/// </summary>
public struct SubSprite
{
    public int left, top, right, bottom;
}

public enum Colours : byte
{
    Begin,
    DarkBlue = Begin,
    PaleGreen,
    Pink,
    Yellow,
    Red,
    LightBlue,
    Green,
    DarkGreen,
    Blue,
    Cream,
    Mauve,
    Purple,
    Orange,
    Brown,
    Grey,
    White,
    End,
    InvalidColour = 0xFF
}

public enum TextColour : ushort
{
    Begin = 0x00,
    FromString = 0x00,
    Blue = 0x00,
    Silver = 0x01,
    Gold = 0x02,
    Red = 0x03,
    Purple = 0x04,
    LightBrown = 0x05,
    Orange = 0x06,
    Green = 0x07,
    Yellow = 0x08,
    DarkGreen = 0x09,
    Cream = 0x0A,
    Brown = 0x0B,
    White = 0x0C,
    LightBlue = 0x0D,
    Grey = 0x0E,
    DarkBlue = 0x0F,
    Black = 0x10,
    End,
    Invalid = 0xFF,

    IsPaletteColour = 0x100, // Colour value is already a real palette colour index, not an index of a StringColour
    NoShade = 0x200, // Do not add shading to this text colour
    Forced = 0x400, // Ignore colour changes from strings

    ColourMask = 0xFF, // Mask to test if TextColour (without flags) is within limits
    FlagsMask = 0x700 // Mask to test if TextColour (with flags) is within limits
}

/// <summary>
/// Define the operation GfxFillRect platform
/// </summary>
public enum FillRectMode : byte
{
    Opaque, // Fill rectangle with a single colour
    Checker, // Draw only every second pixel, used for greying-out
    Recolour // Apply a recolour sprite to the screen context
}

/// <summary>
/// Palettes OpenTTD supports.
/// </summary>
public enum PaletteType : byte
{
    DOS, // Use the DOS palette
    Windows // Use the Windows palette
}

/// <summary>
/// Types of sprites that might be loaded.
/// </summary>
public enum SpriteType : byte
{
    Normal = 0, // The most basic (normal) sprite
    MapGen = 1, // Special sprite for the map generator
    Font = 2, // A sprite used for fonts
    Recolour = 3, // Recolour sprite
    Invalid = 4 // Pseudosprite or other unusable sprite, used only internally
}

/// <summary>
/// Information about the currently used palette.
/// </summary>
public struct Palette
{
    public Colour[] palette = new Colour[256]; // Current palette, entry 0 has to be always fully transparent!
    public int firstDirty;
    public int countDirty;

    public Palette() { }
}

/// <summary>
/// Modes for 8bpp support
/// </summary>
public enum Support8Bpp : byte
{
    None = 0, // No support for 8bpp by OS or hardware, force 32bpp blitters
    System, // No 8bpp support by hardware, do not try to use 8bpp video modes or hardware palettes
    Hardware // Full 8bpp support by OS and hardware
}

/// <summary>
/// How to align the to-be-drawn text
/// </summary>
public enum StringAlignment : byte
{
    Left = 0 << 0, // Left align the text
    HorCenter = 1 << 0, // Horizontally center the text
    Right = 2 << 0, // Right align the text (must be a single bit)
    HorMask = 3 << 0, // Mask for horizontal alignment

    Top = 0 << 2, // Top align the text
    VertCenter = 1 << 2, // Vertically center the text
    Bottom = 2 << 2, // Bottom align the text
    VertMask = 3 << 2, // Mask for vertical alignment

    Center = HorCenter | VertCenter, // Center both horizontally and vertically

    Force = 1 << 4, // Force the alignment, i.e. don't swap for RTL languages
}