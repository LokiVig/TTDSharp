namespace OpenTTD;

/// <summary>
/// Data structure defining a sprite.
/// </summary>
public struct Sprite
{
    public ushort height; // Height of the sprite
    public ushort width; // Width of the sprite
    public short xOffs; // Number of pixels to shift the sprite to the right
    public short yOffs; // Number of pixels to shift the sprite downwards
    public byte[] data; // Sprite data
}

public enum SpriteCacheCtrlFlags : byte
{
    AllowZoomMin1xPal = 0, // Allow use of sprite min zoom setting at 1x in palette mode
    AllowZoomMin1x32bpp = 1, // Allow use of sprite min zoom setting at 1x in 32bpp mode
    AllowZoomMin2xPal = 2, // Allow use of sprite min zoom setting at 2x in palette mode
    AllowZoomMin2x32bpp = 3, // Allow use of sprite min zoom setting at 2x in 32bpp mode
}