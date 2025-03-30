global using ScriptConfigFlags = OpenTTD.Core.EnumBitSet<OpenTTD.Script.ScriptConfigFlag, byte>;
global using LabelMapping = System.Collections.Generic.Dictionary<int, string>;
global using ScriptConfigItemList = System.Collections.Generic.List<OpenTTD.Script.ScriptConfigItem>;
global using SettingValueList = System.Collections.Generic.Dictionary<string, int>;

using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace OpenTTD.Script;

public static partial class Script
{
    // Maximum of 10 digits for MIN / MAX_INT32, 1 for the sign and 1 for '\0'
    public const int INT32_DIGITS_WITH_SIGN_AND_TERMINATION = 10 + 1 + 1;

    public static (string, string) GetValueParams( ScriptConfigItem configItem, int value )
    {
        if ( ( configItem.flags & ScriptConfigFlag.Boolean ) != 0 )
        {
            return (value != 0 ? STR_CONFIG_SETTING_ON : STR_CONFIG_SETTING_OFF, string.Empty);
        }

        string it = configItem.labels.GetValueOrDefault( value );

        if ( it != configItem.labels.Last().Value )
        {
            return (STR_JUST_RAW_STRING, it);
        }

        return (STR_JUST_INT, value);
    }
}

/// <summary>
/// Flags for Script settings.
/// </summary>
public enum ScriptConfigFlag : byte
{
    // Unused flag 0x1
    Boolean = 1, // This value is a boolean (either 0 (false) or 1 (true)).
    InGame = 2, // This setting can be changed while the Script is running
    Developer = 3, // This setting will only be visible when the Script development tools are active
}

/// <summary>
/// Info about a single Script setting.
/// </summary>
public struct ScriptConfigItem
{
    public string name; // The name of the configuration setting
    public string description; // The description of the configuration setting
    public int minValue = 0; // The minimal value this configuration setting can have
    public int maxValue = 1; // The maximal value this configuration setting can have
    public int defaultValue = 0; // The default value of this configuration setting
    public int stepSize = 1; // The step size in the GUI
    public ScriptConfigFlag flags; // Flags for the configuration setting
    public LabelMapping labels; // Text labels for the integer value
    public bool completeLabels = false; // True if all values have a label

    public ScriptConfigItem() { }

    /// <summary>
    /// Get string to display this setting in the configuration interface.
    /// </summary>
    /// <param name="value">Current value.</param>
    /// <returns>String to display.</returns>
    public string GetString( int value )
    {
        (string, string) param = Script.GetValueParams( this, value );

        return string.IsNullOrEmpty( description )
            ? GetString( STR_JUST_STRING1, param.Item1, param.Item2 )
            : GetString( STR_AI_SETTINGS_SETTING, description, param.Item1, param.Item2 );
    }

    public TextColour GetColour()
    {
        return string.IsNullOrEmpty( description ) ? TextColour.Orange : TextColour.LightBlue;
    }
}

/// <summary>
/// Script settings.
/// </summary>
public class ScriptConfig
{
    protected string name; // Name of the Script
    protected int version; // Version of the Script
    protected ScriptInfo info; // ScriptInfo object for related to this Script version
    protected SettingValueList settings; // List with all setting=>value pairs that are configurable for this Script
    protected ScriptConfigItemList configList; // List with all settings defined by this Script
    protected ScriptInstance.ScriptData toLoadData; // Data to load after the Script start

    public ScriptConfig()
    {
        version = -1;
        info = null;
        toLoadData = null;
    }

    /// <summary>
    /// Create a new Script config that is a copy of an existing config.
    /// </summary>
    /// <param name="other">The object to copy.</param>
    public ScriptConfig( ScriptConfig other )
    {
        name = other.name;
        info = other.info;
        version = other.version;
        toLoadData = new ScriptInstance.ScriptData();

        for ( int i = 0; i < other.settings.Count; i++ )
        {
            string key = other.settings.ElementAt( i ).Key;
            int value = other.settings.ElementAt( i ).Value;
            settings.Add( key, value );
        }
    }

    /// <summary>
    /// Delete a Script configuration.
    /// </summary>
    ~ScriptConfig()
    {
        ResetSettings();
        toLoadData = new ScriptInstance.ScriptData();
    }

    /// <summary>
    /// Set another Script to be loaded in this slot.
    /// </summary>
    /// <param name="name">The name of this Script.</param>
    /// <param name="version">The version of the script to load, or -1 for latest.</param>
    /// <param name="forceExactMatch">If <see langword="true"/> try to find the exact same version as specified. If <see langword="false"/> any compatible version is ok.</param>
    public void Change( string? name, int version = -1, bool forceExactMatch = false )
    {
        if ( !string.IsNullOrEmpty( name ) )
        {
            this.name = name;
            info = FindInfo( this.name, version, forceExactMatch );
        }
        else
        {
            info = null;
        }

        this.version = ( info == null ) ? -1 : info.GetVersion();
        configList = new ScriptConfigItemList();
        toLoadData = new ScriptInstance.ScriptData();

        ClearConfigList();
    }

    /// <summary>
    /// Get the ScriptInfo linked to this ScriptConfig.
    /// </summary>
    public ScriptInfo GetInfo()
    {
        return info;
    }

    /// <summary>
    /// Get the config list for this ScriptConfig.
    /// </summary>
    public ScriptConfigItemList GetConfigList()
    {
        if ( info != null )
        {
            info.GetConfigList();
        }

        if ( configList == null )
        {
            configList = new ScriptConfigItemList();
        }

        return configList;
    }

    /// <summary>
    /// Where to get the config from, either default (depends on the current game<br/>
    /// mode) or force either newgame or normal.
    /// </summary>
    public enum ScriptSettingSource : byte
    {
        Default, // Get the Script config from the current game mode
        ForceNewgame, // Get the newgame Script config
        ForceGame // Get the script config from the current game
    }

    /// <summary>
    /// As long as the default of a setting has not been changed, the value of<br/>
    /// the setting is not stored. This is to allow changing the difficulty setting<br/>
    /// without having to reset the script's config. However, when a setting may<br/>
    /// not be changed in game, we must "anchor" this value to what the setting<br/>
    /// would be at the time of starting. Otherwise changing the difficulty<br/>
    /// setting would change the setting's value (which isn't allowed).
    /// </summary>
    public void AnchorUnchangeableSettings()
    {
        foreach ( ScriptConfigItem item in GetConfigList() )
        {
            if ( ( item.flags & ScriptConfigFlag.InGame ) != 0 )
            {
                SetSetting( item.name, GetSetting( item.name ) );
            }
        }
    }

    /// <summary>
    /// Get the value of a setting for this config. It might fallback to its<br/>
    /// 'info' to find the default value (if not set or if not-custom difficulty<br/>
    /// level).
    /// </summary>
    /// <returns>The (default) value of the setting, or -1 if the setting was not found.</returns>
    public int GetSetting( string name )
    {
        int it = settings.GetValueOrDefault( name );

        if ( it == settings.Last().Value )
        {
            return info.GetSettingDefaultValue( name );
        }

        return it;
    }

    /// <summary>
    /// Set the value of a setting for this config.
    /// </summary>
    public void SetSetting( string name, int value )
    {
        // You can only set Script specific settings if a Script is selected
        if ( info == null )
        {
            return;
        }

        ScriptConfigItem configItem = info.GetConfigItem( name );

        if ( configItem.Equals( null ) )
        {
            return;
        }

        value = Math.Clamp( value, configItem.minValue, configItem.maxValue );
        settings[name] = value;
    }

    /// <summary>
    /// Reset all settings to their default value.
    /// </summary>
    public void ResetSettings()
    {
        settings = new SettingValueList();
    }

    /// <summary>
    /// Reset only editable settings to their default value.
    /// </summary>
    public void ResetEditableSettings( bool yetToStart )
    {
        if ( info == null )
        {
            ResetSettings();
            return;
        }

        for ( KeyValuePair<string, int> it = settings.First(); !it.Equals(settings.Last()); )
        {
            ScriptConfigItem configItem = info.GetConfigItem( it );
            Debug.Assert( !configItem.Equals( null ) );

            bool editable = yetToStart || ( configItem.flags & ScriptConfigFlag.InGame ) != 0;
            bool visible = Settings.settingsClient.gui.aiDeveloperTools || ( configItem.flags & ScriptConfigFlag.Developer ) == 0;

            if ( editable && visible )
            {
                //it = settings.
                // ^ This is impossible!!! ^
            }
            else
            {
                it++;
            }
        }
    }

    /// <summary>
    /// Is this config attached to a Script? In other words, is there a Script<br/>
    /// that is assigned to this slot.
    /// </summary>
    public bool HasScript()
    {
        return info != null;
    }

    /// <summary>
    /// Get the name of the Script.
    /// </summary>
    public string GetName()
    {
        return name;
    }

    /// <summary>
    /// Get the version of the Script.
    /// </summary>
    public int GetVersion()
    {
        return version;
    }

    /// <summary>
    /// Convert a string which is stored in the config file or savegames to<br/>
    /// custom settings of this Script.
    /// </summary>
    public void StringToSettings( string value )
    {
        string toProcess = value;

        for (; ; )
        {
            // Analyze the string ('name=value,name=value\0')
            ulong pos = (ulong)toProcess.IndexOf('=');

            if ( pos == ulong.MaxValue )
            {
                return;
            }

            string itemName = toProcess.Substring( 0, (int)pos );
            toProcess.Remove( (int)pos + 1 );
            pos = (ulong)toProcess.IndexOf( ',' );
            int itemValue = 0;
            itemValue = int.Parse( toProcess );

            SetSetting( itemName, itemValue );

            if ( pos == ulong.MaxValue )
            {
                return;
            }

            toProcess.Remove( (int)pos + 1 );
        }
    }

    /// <summary>
    /// Convert the custom settings to a string that can be stored in the config<br/>
    /// file or savegames.
    /// </summary>
    public string SettingsToString()
    {
        if ( settings.Count == 0 )
        {
            return string.Empty;
        }

        string result = string.Empty;

        foreach ( KeyValuePair<string, int> item in settings )
        {
            result = $"{item.Key}={item.Value}";
        }

        // Remove the last ','
        result.Remove( result.Length - 1 );
        return result;
    }

    /// <summary>
    /// Search a textfile next to this script.
    /// </summary>
    /// <param name="type">The type of textfile to search for.</param>
    /// <param name="slot"><see cref="CompanyID"/> to check status of.</param>
    /// <returns>The filename of the textfile.</returns>
    public string GetTextFile( TextFileType type, CompanyID slot )
    {
        if ( slot == CompanyID.Invalid() || GetInfo() == null )
        {
            return string.Empty;
        }

        return GetTextFile( type, ( slot == OWNER_DEITY ) ? GAME_DIR : AI_DIR, GetInfo().GetMainScript() );
    }

    public void SetToLoadData( ScriptInstance.ScriptData data )
    {
        toLoadData = new ScriptInstance.ScriptData( data );
    }

    public ScriptInstance.ScriptData GetToLoadData()
    {
        return toLoadData;
    }

    /// <summary>
    /// Routine that clears the config list.
    /// </summary>
    protected void ClearConfigList()
    {
        settings.Clear();
    }

    /// <summary>
    /// This function should call back to the Scanner in charge of this Config,<br/>
    /// to find the ScriptInfo belonging to a name + version.
    /// </summary>
    protected virtual ScriptInfo FindInfo( string name, int version, bool forceExactMatch )
    {

    }
}