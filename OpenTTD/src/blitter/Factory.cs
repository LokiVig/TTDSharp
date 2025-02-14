global using Blitters = System.Collections.Generic.Dictionary<string, OpenTTD.Blitter.BlitterFactory>; // Map of blitter factories

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenTTD.Blitter;

public class BlitterFactory
{
    public static string iniBlitter;
    public static bool blitterAutoDetected;

    private string name; // The name of the blitter factory
    private string description; // The description of the blitter

    /// <summary>
    /// Construct the blitter, and register it.
    /// </summary>
    /// <param name="name">The name of the blitter.</param>
    /// <param name="description">A longer description for the blitter.</param>
    /// <param name="usable">Whether or not the blitter is usable (on the current computer). For example for disabling SSE blitters when the CPU can't handle them.</param>
    protected BlitterFactory( string name, string description, bool usable = true )
    {
        this.name = name;
        this.description = description;

        if ( usable )
        {
            Blitters blitters = GetBlitters();
            Debug.Assert( blitters[this.name] == blitters.Last().Value );

            // Only add when the blitter is usable, do not bail out or
            // do more special things since the blitters are always
            // instantiated upon start anyhow and freed upon shutdown
            blitters.Add( this.name, this );
        }
        else
        {
            throw new NotImplementedException( "Function \"Debug\" is not yet defined!" );
        }
    }

    ~BlitterFactory()
    {
        GetBlitters().Remove( name );

        if ( GetBlitters().Count == 0 )
        {
            GetBlitters() = null;
        }
    }

    /// <summary>
    /// Get the map with currently known blitters.
    /// </summary>
    /// <returns>The known blitters.</returns>
    private static ref Blitters GetBlitters()
    {
        Blitters sBlitters = new Blitters();
        return ref sBlitters;
    }

    /// <summary>
    /// Get the currently active blitter.
    /// </summary>
    /// <returns>The currently active blitter.</returns>
    private static ref Blitter GetActiveBlitter()
    {
        Blitter sBlitter = null;
        return ref sBlitter;
    }

    /// <summary>
    /// Is the blitter usable with the current drivers and hardware config?
    /// </summary>
    /// <returns><see langword="true"/> if the blitter can be instantiated.</returns>
    protected virtual bool IsUsable()
    {
        return true;
    }

    /// <summary>
    /// Find the requested blitter and return its class.
    /// </summary>
    /// <param name="name">The blitter to select.</param>
    public static Blitter SelectBlitter( string name )
    {
        BlitterFactory b = GetBlitterFactory( name );

        if ( b == null )
        {
            return null;
        }

        Blitter newB = b.CreateInstance();
        GetActiveBlitter() = null;
        GetActiveBlitter() = newB;

        throw new NotImplementedException( "Function \"Debug\" is not yet defined!" );
        return newB;
    }

    /// <summary>
    /// Get the blitter factory with the given name.
    /// </summary>
    /// <param name="name">The blitter factory to select.</param>
    /// <returns>The blitter factory, or <see langword="null"/> when there isn't one with the wanted name.</returns>
    public static BlitterFactory GetBlitterFactory( string name )
    {
#if DEDICATED
        string defaultBlitter = "null";
#elif WITH_COCOA
        string defaultBlitter = "32bpp-anim";
#else
        string defaultBlitter = "8bpp-optimized";
#endif // DEDICATED || WITH_COCOA

        if ( GetBlitters().Count == 0 )
        {
            return null;
        }

        string bName = string.IsNullOrEmpty( name ) ? defaultBlitter : name;

        foreach ( var it in GetBlitters() )
        {
            BlitterFactory b = it.Value;

            if ( bName == b.name )
            {
                return b.IsUsable() ? b : null;
            }
        }

        return null;
    }

    /// <summary>
    /// Get the current active blitter (always set by calling <see cref="SelectBlitter(string)"/>).
    /// </summary>
    public static Blitter GetCurrentBlitter()
    {
        return GetActiveBlitter();
    }

    /// <summary>
    /// Fill a buffer with information about the blitters.
    /// </summary>
    /// <param name="outputIterator">The buffer to fill.</param>
    public static void GetBlittersInfo( List<string> outputIterator )
    {
        int index = 0;

        outputIterator[index] = "List of blitters:\n";

        foreach ( var it in GetBlitters() )
        {
            BlitterFactory b = it.Value;

            outputIterator[index++] = $"\t{b.name}: {b.GetDescription()}\n";
        }
        outputIterator[index++] = "\n";
    }

    /// <summary>
    /// Get the long. human readable name of the <see cref="Blitter"/>-class.
    /// </summary>
    public string GetName()
    {
        return name;
    }

    /// <summary>
    /// Get a nice description of the <see cref="Blitter"/>-class.
    /// </summary>
    public string GetDescription()
    {
        return description;
    }

    /// <summary>
    /// Create an instance of this <see cref="Blitter"/>-class.
    /// </summary>
    public virtual Blitter CreateInstance()
    {
        return null;
    }
}