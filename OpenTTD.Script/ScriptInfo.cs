using OpenTTD.Script.API;

namespace OpenTTD.Script;

public static partial class Script
{
    /// <summary>
    /// The maximum number of operations for saving or loading the data of a script.
    /// </summary>
    public const int MAX_SL_OPS = 100000;

    /// <summary>
    /// The maximum number of operations for initial start of a script.
    /// </summary>
    public const int MAX_CONSTRUCTOR_OPS = 100000;

    /// <summary>
    /// Number of operations to create an instance of a script.
    /// </summary>
    public const int MAX_CREATEINSTANCE_OPS = 100000;

    /// <summary>
    /// Number of operations to get the author and similar information.
    /// </summary>
    public const int MAX_GET_OPS = 1000;

    /// <summary>
    /// Maximum number of operations allowed for getting a particular setting.
    /// </summary>
    public const int MAX_GET_SETTINGS_OPS = 100000;

    public static void CreateDummyInfo( SQVM vm, string type, string dir )
    {

    }

    public static void CreateDummy( SQVM vm, StringID str, string type )
    {

    }
}

/// <summary>
/// All static information from a script like name, version, etc.
/// </summary>
public class ScriptInfo : SimpleCountedObject
{
    protected Squirrel engine = null; // Engine used to register for Squirrel
    protected SQObject SQInstance = new SQObject(); // The Squirrel instance created for this info
    protected ScriptConfigItemList configList = new ScriptConfigItemList(); // List of settings from this script

    private string mainScript = string.Empty; // The full path of the script
    private string tarFile = string.Empty; // If, which tar file the script was in
    private string author = string.Empty; // Author of the script
    private string name = string.Empty; // Full name of the script
    private string shortName = string.Empty; // Short name (4 chars) which uniquely identifies the script
    private string description = string.Empty; // Small description of the script
    private string date = string.Empty; // The date the script was written at
    private string instanceName = string.Empty; // Name of the main class in the script
    private string url = string.Empty; // URL of the script

    private int version = 0; // Version of the script

    private ScriptScanner scanner = null; // ScriptScanner object that was used to scan this script info

    /// <summary>
    /// Get the author of the script.
    /// </summary>
    public string GetAuthor()
    {
        return author;
    }

    /// <summary>
    /// Get the name of the script.
    /// </summary>
    public string GetName()
    {
        return name;
    }

    /// <summary>
    /// Get the 4 character long short name of the script.
    /// </summary>
    public string GetShortName()
    {
        return shortName;
    }

    /// <summary>
    /// Get the description of the script.
    /// </summary>
    public string GetDescription()
    {
        return description;
    }

    /// <summary>
    /// Get the version of the script.
    /// </summary>
    public int GetVersion()
    {
        return version;
    }

    /// <summary>
    /// Get the last-modified date of the script.
    /// </summary>
    public string GetDate()
    {
        return date;
    }

    /// <summary>
    /// Get the name of the instance of the script to create.
    /// </summary>
    public string GetInstanceName()
    {
        return instanceName;
    }

    /// <summary>
    /// Get the website for this script.
    /// </summary>
    public string GetURL()
    {
        return url;
    }

    /// <summary>
    /// Get the filename of the <c>main.nut</c> script.
    /// </summary>
    public string GetMainScript()
    {
        return mainScript;
    }

    /// <summary>
    /// Get the filename of the tar the script is in.
    /// </summary>
    public string GetTarFile()
    {
        return tarFile;
    }

    /// <summary>
    /// Check if a given method exists.
    /// </summary>
    public bool CheckMethod( string name )
    {
        
    }

    /// <summary>
    /// Process the creation of a FileInfo object.
    /// </summary>
    public static SQInteger Constructor( SQVM vm, ScriptInfo info )
    {

    }

    /// <summary>
    /// Get the scanner which has found this <see cref="ScriptInfo"/>.
    /// </summary>
    public virtual ScriptScanner GetScanner()
    {
        return scanner;
    }

    /// <summary>
    /// Get the settings of the script.
    /// </summary>s
    public bool GetSettings()
    {

    }

    /// <summary>
    /// Get the config list for this script.
    /// </summary>
    public ScriptConfigItemList GetConfigList()
    {

    }

    /// <summary>
    /// Get the description of a certain script config option.
    /// </summary>
    public ScriptConfigItem GetConfigItem( string name )
    {

    }

    /// <summary>
    /// Set a setting.
    /// </summary>
    public SQInteger AddSetting( SQVM vm )
    {

    }
    
    /// <summary>
    /// Add labels for a setting.
    /// </summary>
    public SQInteger AddLabels( SQVM vm )
    {

    }

    /// <summary>
    /// Get the default value for a setting.
    /// </summary>
    public int GetSettingDefaultValue( string name )
    {

    }

    /// <summary>
    /// Can this script be selected by developers only?
    /// </summary>
    public virtual bool IsDeveloperOnly()
    {
        return false;
    }
}