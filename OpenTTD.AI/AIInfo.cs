using System;
using System.Linq;
using System.Reflection;

using OpenTTD.Script;

namespace OpenTTD.AI;

public static partial class AI
{
    /// <summary>
    /// Check if the API version provided by the AI is supported.
    /// </summary>
    /// <param name="apiVersion">The API version as provided by the AI.</param>
    public static bool CheckAPIVersion( string apiVersion )
    {
        return AIInfo.ApiVersions.Contains( apiVersion );
    }


}

/// <summary>
/// All static information from an AI like name, version, etc.
/// </summary>
public class AIInfo : ScriptInfo
{
    // All valid API versions, in order
    public static readonly string[] ApiVersions = { "0.7", "1.0", "1.1", "1.2", "1.3", "1.4", "1.5", "1.6", "1.7", "1.8", "1.9", "1.10", "1.11", "12", "13", "14", "15" };

    private int minLoadableVersion; // The AI can load savegame data if the version is equal or greater than this
    private bool useAsRandom; // Should this AI be used when the user wants a "random AI"?
    private string apiVersion; // API version used by this AI

    public AIInfo()
    {

    }

    /// <summary>
    /// Register the functions of this class.
    /// </summary>
    public static void RegisterAPI( Squirrel engine )
    {
        DefSQClass<AIInfo> sqAIInfo = new DefSQClass<AIInfo>( ScriptType.AI, "AIInfo" );
        sqAIInfo.PreRegister( engine );
        sqAIInfo.AddConstructor( engine, 1, "x" );
        sqAIInfo.DefSQAdvancedMethod<SQVM, SQInteger>( engine, AddSetting, "AddSetting" );
        sqAIInfo.DefSQAdvancedMethod<SQVM, SQInteger>( engine, AddLabels, "AddLabels" );
        sqAIInfo.DefSQConst( engine, new ScriptConfigFlags(), "CONFIG_NONE" );
        sqAIInfo.DefSQConst( engine, new ScriptConfigFlags(), "CONFIG_RANDOM" ); // Deprecated, mapped to NONE
        sqAIInfo.DefSQConst( engine, new ScriptConfigFlags( ScriptConfigFlag.Boolean ), "CONFIG_BOOLEAN" );
        sqAIInfo.DefSQConst( engine, new ScriptConfigFlags( ScriptConfigFlag.InGame ), "CONFIG_INGAME" );
        sqAIInfo.DefSQConst( engine, new ScriptConfigFlags( ScriptConfigFlag.Developer ), "CONFIG_DEVELOPER" );

        // Pre 1.2 had an AI prefix
        sqAIInfo.DefSQConst( engine, new ScriptConfigFlags(), "AICONFIG_NONE" );
        sqAIInfo.DefSQConst( engine, new ScriptConfigFlags(), "AICONFIG_RANDOM" ); // Deprecated, mapped to NONE
        sqAIInfo.DefSQConst( engine, new ScriptConfigFlags( ScriptConfigFlag.Boolean ), "AICONFIG_BOOLEAN" );
        sqAIInfo.DefSQConst( engine, new ScriptConfigFlags( ScriptConfigFlag.InGame ), "AICONFIG_INGAME" );

        sqAIInfo.PostRegister( engine );
        engine.AddMethod( "RegisterAI", AIInfo.Constructor, 2, "tx" );
        engine.AddMethod( "RegisterDummyAI", AIInfo.DummyConstructor, 2, "tx" );
    }

    /// <summary>
    /// Create an AI, using this AIInfo as start-template.
    /// </summary>
    public static SQInteger Constructor( SQVM vm )
    {
        // Get the AI info
        SQUserPointer instance = null;
    }

    /// <summary>
    /// Create a dummy AI.
    /// </summary>
    public static SQInteger DummyConstructor( SQVM vm )
    {

    }

    /// <summary>
    /// Check if we can start this AI.
    /// </summary>
    public bool CanLoadFromVersion( int version )
    {

    }

    /// <summary>
    /// Use this AI as a random AI.
    /// </summary>
    public bool UseAsRandomAI()
    {
        return useAsRandom;
    }

    /// <summary>
    /// Get the API version this AI is written for.
    /// </summary>
    public string GetAPIVersion()
    {
        return apiVersion;
    }
}

/// <summary>
/// All static information from an AI library like name, version, etc.
/// </summary>
public class AILibrary : ScriptInfo
{
    private string category; // The category this library is in

    public AILibrary() : base()
    {

    }

    /// <summary>
    /// Register the functions of this class.
    /// </summary>
    public static void RegisterAPI( Squirrel engine )
    {

    }

    /// <summary>
    /// Create an AI, using this AIInfo as start-template.
    /// </summary>
    public static SQInteger Constructor( SQVM vm )
    {

    }

    /// <summary>
    /// Get the category this library is in.
    /// </summary>
    public string GetCategory()
    {
        return category;
    }
}