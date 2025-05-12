using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenTTD;

/// <summary>
/// All data for a single hotkey. The name (for saving / loading a configfile),<br/>
/// a list of keycodes and a number to help identifying this hotkey.
/// </summary>
public struct Hotkey
{
    public string name;
    public int num;
    public List<ushort> keycodes = new List<ushort>();

    public Hotkey( ushort defaultKeycode, string name, int num )
    {
        this.name = name;
        this.num = num;
        
        if ( defaultKeycode != 0 )
        {
            AddKeycode( defaultKeycode );
        }
    }

    public Hotkey( ushort[] defaultKeycodes, string name, int num )
    {
        this.name = name;
        this.num = num;

        foreach ( ushort keycode in defaultKeycodes )
        {
            AddKeycode( keycode );
        }
    }

    public void AddKeycode( ushort keycode )
    {
        keycodes.Add( keycode );
    }

    public static bool IsQuitKey( ushort keycode )
    {

    }

    /// <summary>
    /// Parse a string to the keycodes it represents.
    /// </summary>
    /// <param name="hotkey">The hotkey object to add the keycodes to.</param>
    /// <param name="value">The string to parse.</param>
    public static void ParseHotkeys( Hotkey hotkey, string value )
    {
        char[] start = value.ToCharArray();
        int startIndex = 0;

        while ( start[startIndex] != '\0' )
        {
            char[] end = start;
            int endIndex = 0;

            while ( end[endIndex] != '\0' && end[endIndex] != ',' )
            {
                endIndex++;
            }

            ushort keycode = KeycodeNames.ParseKeycode( start, end );

            if ( keycode != 0 )
            {
                hotkey.AddKeycode( keycode );
            }

            start[startIndex] = ( end[endIndex] == ',' ) ? end[endIndex + 1] : end[endIndex];
        }
    }
}

/// <summary>
/// List of hotkeys for a window.
/// </summary>
public struct HotkeyList
{
    public static string hotkeysFile;
    public static List<HotkeyList> hotkeyLists = null;

    public GlobalHotkeyHandlerFunc globalHotkeyHandler;

    private string iniGroup;
    private Hotkey[] items;

    public delegate EventState GlobalHotkeyHandlerFunc( int hotkey );

    public HotkeyList( string iniGroup, Hotkey[] items, GlobalHotkeyHandlerFunc globalHotkeyHandler = null )
    {
        this.globalHotkeyHandler = globalHotkeyHandler;
        this.iniGroup = iniGroup;
        this.items = items;

        if ( hotkeyLists == null )
        {
            hotkeyLists = new List<HotkeyList>();
        }

        hotkeyLists.Add( this );
    }

    /// <summary>
    /// Load <see cref="HotkeyList"/> from <see cref="IniFile"/>.
    /// </summary>
    /// <param name="ini">IniFile to load from.</param>
    public void Load( IniFile ini )
    {
        IniGroup group = ini.GetGroup( iniGroup );

        if ( group == null )
        {
            return;
        }

        foreach ( Hotkey hotkey in items )
        {
            IniItem item = group.GetItem( hotkey.name );

            if ( item != null )
            {
                hotkey.keycodes.Clear();

                if ( item.value != null )
                {
                    Hotkey.ParseHotkeys( hotkey, item.value.ToString() );
                }
            }
        }
    }

    /// <summary>
    /// Save HotkeyList to IniFile.
    /// </summary>
    /// <param name="ini">IniFile to save to.</param>
    public void Save( IniFile ini )
    {
        IniGroup group = ini.GetOrCreateGroup( iniGroup );

        foreach ( Hotkey hotkey in items )
        {
            IniItem item = group.GetOrCreateItem( hotkey.name );
            item.SetValue( KeycodeNames.SaveKeycodes( hotkey ) );
        }
    }

    /// <summary>
    /// Check if a keycode is bound to something.
    /// </summary>
    /// <param name="keycode">The keycode that was pressed.</param>
    /// <param name="globalOnly">Limit the search to hotkeys defined as 'global'.</param>
    /// <returns>The number of the matching hotkey or -1.</returns>
    public int CheckMatch( ushort keycode, bool globalOnly = false )
    {
        foreach ( Hotkey hotkey in items )
        {
            ushort begin = hotkey.keycodes.First();
            ushort end = hotkey.keycodes.Last();

            // TODO: C++ implementation: if (std::find(begin, end, keycode | WKC_GLOBAL_HOTKEY) != end || (!global_only && std::find(begin, end, keycode) != end))
            // if ()
            return hotkey.num;
        }

        return -1;
    }

    public static void LoadHotkeysFromConfig()
    {
        SaveLoadHotkeys( false );
    }

    public static void SaveHotkeysToConfig()
    {
        SaveLoadHotkeys( true );
    }

    public static void HandleGlobalHotkeys( sbyte key, ushort keycode )
    {
        foreach ( HotkeyList list in hotkeyLists )
        {
            if ( list.globalHotkeyHandler == null )
            {
                continue;
            }

            int hotkey = list.CheckMatch( keycode, true );

            if ( hotkey >= 0 && ( list.globalHotkeyHandler( hotkey ) == EventState.Handled ) )
            {
                return;
            }
        }
    }

    public static void SaveLoadHotkeys( bool save )
    {
        IniFile ini = new IniFile();
        ini.LoadFromDisk( hotkeysFile, NoDirectory );

        foreach ( HotkeyList list in hotkeyLists )
        {
            if ( save )
            {
                list.Save( ini );
            }
            else
            {
                list.Load( ini );
            }
        }

        if ( save )
        {
            ini.SaveToDisk( hotkeysFile );
        }
    }
}

/// <summary>
/// String representation of a keycode.
/// </summary>
public struct KeycodeNames
{
    public string name; // Name of the keycode
    public WindowsKeyCodes keycode; // The keycode

    public KeycodeNames( string name, WindowsKeyCodes keycode )
    {
        this.name = name;
        this.keycode = keycode;
    }

    public static KeycodeNames[] keycodeToName =
    {
        new KeycodeNames("SHIFT", WindowsKeyCodes.Shift),
        new KeycodeNames("CTRL", WindowsKeyCodes.Ctrl),
        new KeycodeNames("ALT", WindowsKeyCodes.Alt),
        new KeycodeNames("META", WindowsKeyCodes.Meta),
        new KeycodeNames("GLOBAL", WindowsKeyCodes.GlobalHotkey),
        new KeycodeNames("ESC", WindowsKeyCodes.Esc),
        new KeycodeNames("BACKSPACE", WindowsKeyCodes.Backspace),
        new KeycodeNames("INS", WindowsKeyCodes.Insert),
        new KeycodeNames("DEL", WindowsKeyCodes.Delete),
        new KeycodeNames("PAGEUP", WindowsKeyCodes.PageUp),
        new KeycodeNames("PAGEDOWN", WindowsKeyCodes.PageDown),
        new KeycodeNames("END", WindowsKeyCodes.End),
        new KeycodeNames("HOME", WindowsKeyCodes.Home),
        new KeycodeNames("RETURN", WindowsKeyCodes.Return),
        new KeycodeNames("SPACE", WindowsKeyCodes.Space),
        new KeycodeNames("F1", WindowsKeyCodes.F1),
        new KeycodeNames("F2", WindowsKeyCodes.F2),
        new KeycodeNames("F3", WindowsKeyCodes.F3),
        new KeycodeNames("F4", WindowsKeyCodes.F4),
        new KeycodeNames("F5", WindowsKeyCodes.F5),
        new KeycodeNames("F6", WindowsKeyCodes.F6),
        new KeycodeNames("F7", WindowsKeyCodes.F7),
        new KeycodeNames("F8", WindowsKeyCodes.F8),
        new KeycodeNames("F9", WindowsKeyCodes.F9),
        new KeycodeNames("F10", WindowsKeyCodes.F10),
        new KeycodeNames("F11", WindowsKeyCodes.F11),
        new KeycodeNames("F12", WindowsKeyCodes.F12),
        new KeycodeNames("BACKQUOTE", WindowsKeyCodes.Backqoute),
        new KeycodeNames("PAUSE", WindowsKeyCodes.Pause),
        new KeycodeNames("NUM_DIV", WindowsKeyCodes.NumDiv),
        new KeycodeNames("NUM_MUL", WindowsKeyCodes.NumMul),
        new KeycodeNames("NUM_MINUS", WindowsKeyCodes.NumMinus),
        new KeycodeNames("NUM_PLUS", WindowsKeyCodes.NumPlus),
        new KeycodeNames("NUM_ENTER", WindowsKeyCodes.NumEnter),
        new KeycodeNames("NUM_DOT", WindowsKeyCodes.NumDecimal),
        new KeycodeNames("SLASH", WindowsKeyCodes.Slash),
        new KeycodeNames("/", WindowsKeyCodes.Slash), // OBSOLETE -- Use SLASH
        new KeycodeNames("SEMICOLON", WindowsKeyCodes.Semicolon),
        new KeycodeNames(";", WindowsKeyCodes.Semicolon), // OBSOLETE -- Use SEMICOLON
        new KeycodeNames("EQUALS", WindowsKeyCodes.Equals),
        new KeycodeNames("=", WindowsKeyCodes.Equals), // OBSOLETE -- Use EQUALS
        new KeycodeNames("L_BRACKET", WindowsKeyCodes.LBracket),
        new KeycodeNames("[", WindowsKeyCodes.LBracket), // OBSOLETE -- Use L_BRACKET
        new KeycodeNames("BACKSLASH", WindowsKeyCodes.Backslash),
        new KeycodeNames("\\", WindowsKeyCodes.Backslash), // OBSOLETE -- Use BACKSLASH
        new KeycodeNames("R_BRACKET", WindowsKeyCodes.RBracket),
        new KeycodeNames("]", WindowsKeyCodes.RBracket), // OBSOLETE -- Use R_BRACKET
        new KeycodeNames("SINGLEQUOTE", WindowsKeyCodes.SingleQuote),
        new KeycodeNames("'", WindowsKeyCodes.SingleQuote), // OBSOLETE -- Use SINGLEQUOTE
        new KeycodeNames("COMMA", WindowsKeyCodes.Comma),
        new KeycodeNames("PERIOD", WindowsKeyCodes.Period),
        new KeycodeNames(".", WindowsKeyCodes.Period), // OBSOLETE -- Use PERIOD
        new KeycodeNames("MINUS", WindowsKeyCodes.Minus),
        new KeycodeNames("-", WindowsKeyCodes.Minus), // OBSOLETE -- Use MINUS
    };

    /// <summary>
    /// Try to parse a single part of a keycode.
    /// </summary>
    /// <param name="start">Start of the string to parse.</param>
    /// <param name="end">End of the string to parse.</param>
    /// <returns>A keycode if a match is found or 0.</returns>
    public static ushort ParseCode( char[] start, char[] end )
    {
        int startIndex = 0;
        int endIndex = 0;

        Debug.Assert( start.Length <= end.Length );

        while ( start[startIndex] < end[endIndex] && start[startIndex] == ' ' )
        {
            startIndex++;
        }

        while ( end[endIndex] > start[startIndex] && end[endIndex] == ' ' )
        {
            endIndex--;
        }

        string str = new string( new string( start ) + new string( end ) );

        foreach ( KeycodeNames kn in keycodeToName )
        {
            if ( string.Compare( str, kn.name ) == 0 )
            {
                return (ushort)kn.keycode;
            }
        }

        if ( end[endIndex] - start[startIndex] == 1 )
        {
            if ( start[startIndex] >= 'a' && start[startIndex] <= 'z' )
            {
                return (ushort)(start[startIndex] - ( 'a' - 'A' ));
            }

            // Ignore invalid keycodes
            if ( start[startIndex] < 128 )
            {
                return start[startIndex];
            }
        }

        return 0;
    }

    /// <summary>
    /// Parse a string representation of a keycode.
    /// </summary>
    /// <param name="start">Start of the input.</param>
    /// <param name="end">End of the input.</param>
    /// <returns>A valid keycode or 0.</returns>
    public static ushort ParseKeycode( char[] start, char[] end )
    {
        int startIndex = 0;
        int endIndex = 0;

        Debug.Assert( start.Length <= end.Length );

        ushort keycode = 0;

        for (; ; )
        {
            char[] cur = start;
            int curIndex = 0;

            while ( cur[curIndex] != '+' && cur != end )
            {
                curIndex++;
            }

            ushort code = ParseCode( start, cur );

            if ( code == 0 )
            {
                return 0;
            }

            if ( ( code & (int)WindowsKeyCodes.SpecialKeys ) != 0 )
            {
                // Some completely wrong keycode we don't support
                if ( ( code & ~(int)WindowsKeyCodes.SpecialKeys ) != 0 )
                {
                    return 0;
                }

                keycode |= code;
            }
            else
            {
                // Ignore the code if it has more than 1 letter
                if ( ( keycode & ~(int)WindowsKeyCodes.SpecialKeys ) != 0 )
                {
                    return 0;
                }

                keycode |= code;
            }

            if ( cur == end )
            {
                break;
            }

            Debug.Assert( cur.Length < end.Length );

            startIndex = cur.Length + 1;
        }

        return keycode;
    }

    /// <summary>
    /// Convert a hotkey to it's string representation so it can be written to the<br/>
    /// config file. Separate parts of the keycode (like "CTRL" and "F1" are split<br/>
    /// by a '+').
    /// </summary>
    /// <param name="keycode"></param>
    /// <returns></returns>
    public static string KeycodeToString( ushort keycode )
    {
        string str = string.Empty;

        if ( ( keycode & (int)WindowsKeyCodes.GlobalHotkey ) != 0 )
        {
            str += "GLOBAL";
        }

        if ( ( keycode & (int)WindowsKeyCodes.Shift ) != 0 )
        {
            if ( !string.IsNullOrEmpty( str ) )
            {
                str += "+";
            }

            str += "SHIFT";
        }

        if ( ( keycode & (int)WindowsKeyCodes.Ctrl ) != 0 )
        {
            if ( !string.IsNullOrEmpty( str ) )
            {
                str += "+";
            }

            str += "CTRL";
        }

        if ( ( keycode & (int)WindowsKeyCodes.Alt ) != 0 )
        {
            if ( !string.IsNullOrEmpty( str ) )
            {
                str += "+";
            }

            str += "ALT";
        }

        if ( ( keycode & (int)WindowsKeyCodes.Meta ) != 0 )
        {
            if ( !string.IsNullOrEmpty( str ) )
            {
                str += "+";
            }

            str += "META";
        }

        if ( !string.IsNullOrEmpty( str ) )
        {
            str += "+";
        }

        keycode = (ushort)(keycode & ~(int)WindowsKeyCodes.SpecialKeys);

        foreach ( KeycodeNames kn in keycodeToName )
        {
            if ( (ushort)kn.keycode == keycode )
            {
                str += kn.name;
                return str;
            }
        }

        Debug.Assert( keycode < 128 );
        str += keycode;
        return str;
    }

    /// <summary>
    /// Convert all keycodes attached to a hotkey to a single string. If multiple<br/>
    /// keycodes are attached to the hotkey they are split by a comma.
    /// </summary>
    /// <param name="hotkey">The keycodes of this hotkey need to be converted to a string.</param>
    /// <returns>A string representation of all keycodes.</returns>
    public static string SaveKeycodes( Hotkey hotkey )
    {
        string str = string.Empty;

        foreach ( WindowsKeyCodes keycode in hotkey.keycodes )
        {
            if ( !string.IsNullOrEmpty( str ) )
            {
                str += ",";
            }

            str += KeycodeToString( (ushort)keycode );
        }

        return str;
    }
}