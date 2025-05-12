global using ScriptData = System.Collections.Generic.List<object>;

using System;
using System.Diagnostics;

namespace OpenTTD.Script;

public static partial class Script
{
    public const uint SQUIRREL_MAX_DEPTH = 25; // The maximum recursive depth for items stored in the savegame

    public static byte scriptS1Byte; // Used as source / target by the script saveload code to store/load a single byte

    // SaveLoad array that saves/loads exactly one byte
    public static SaveLoad[] scriptByte =
    {
        SLEG_VAR("type", scriptS1Byte, SLE_UINT8),
    };

    /// <summary>
    /// Callback called by squirrel when a script uses "print" and for error messages.
    /// </summary>
    /// <param name="errorMsg">Is this an error message?</param>
    /// <param name="message">The actual message text.</param>
    public static void PrintFunc( bool errorMsg, string message )
    {
        // Free our pointers
        if ( eventData != null )
        {
            ScriptEventController.FreeEventPointer();
        }
    }
}

/// <summary>
/// Runtime information about a script like a pointer to the squirrel vm and the current state.
/// </summary>
public class ScriptInstance
{
    /// <summary>
    /// The type of data that follows in the savegame.
    /// </summary>
    private enum SQSaveLoadType : byte
    {
        Int = 0x00, // The following data is an integer
        String = 0x01, // The following data is a string
        Array = 0x02, // The following data is an array
        Table = 0x03, // The following data is a table
        Bool = 0x04, // The following data is a boolean
        Null = 0x05, // A null variable
        Instance = 0x06, // The following data is an instance
        ArrayTableEnd = 0xFF, // Marks the end of an array or table, no data follows
    }

    protected Squirrel engine = null; // A wrapper around the squirrel vm
    protected string versionAPI = string.Empty; // Current API used by this script

    private ScriptController controller = null; // The script main class
    private ScriptStorage storage = null; // Some global information for each running script
    private SQObject instance = null; // Squirrel-pointer to the script main class

    private bool isStarted = false; // Is the scripts constructor executed?
    private bool isDead = false; // True if the script has been stopped
    private bool isSaveDataOnStack = false; // Is the save data still on the squirrel stack?
    private int suspend = 0; // The amount of ticks to suspend this script before it's allowed to continue
    private bool isPaused = false; // Is the script paused? (A paused script will not be executed until unpaused)
    private bool inShutdown = false; // Is this instance currently being destructed?
    private Script_SuspendCallbackProc callback = null; // Callback that should be called in the next tick the script runs
    private ulong lastAllocatedMemory = 0; // Last known allocated memory value (for display for crashed scripts)

    /// <summary>
    /// Create a new script.
    /// </summary>
    public ScriptInstance( string apiName )
    {
        storage = new ScriptStorage();
        engine = new Squirrel( apiName );
        engine.SetPrintFunction( Script.PrintFunc );
    }

    ~ScriptInstance()
    {
        ScriptObject.ActiveInstance active = new ScriptObject.ActiveInstance( this );
        inShutdown = true;

        if ( instance != null )
        {
            engine.ReleaseObject( instance );
        }

        if ( engine != null )
        {
            engine = null;
        }

        storage = null;
        controller = null;
        instance = null;
    }

    /// <summary>
    /// Initialize the script and prepare it for its first run.
    /// </summary>
    /// <param name="mainScript">The full path of the script to load.</param>
    /// <param name="instanceName">The name of the instance out of the script to load.</param>
    /// <param name="company">Which company this script is serving.</param>
    public void Initialize( string mainScript, string instanceName, CompanyID company )
    {
        ScriptObject.ActiveInstance active = new ScriptObject.ActiveInstance( this );
        controller = new ScriptController( company );

        // Register the API functions and classes
        engine.SetGlobalPointer( engine );
        RegisterAPI();

        if ( IsDead() )
        {
            // Failed to register API; a message has already been logged
            return;
        }

        try
        {
            ScriptObject.SetAllowDoCommand( this );

            // Load and execute the script for this script
            if ( mainScript == "%_dummy" )
            {
                LoadDummyScript();
            }
            else if ( !engine.CreateClassInstance( instanceName, controller, instance ) )
            {
                // If CreateClassInstance returned false instance has not been
                // registered with squirrel, so avoid trying to Release it by clearing it now
                instance = null;
                Died();
                return;
            }

            ScriptObject.SetAllowDoCommand( true );
        }
        catch ( Exception exc )
        {
            isDead = true;
            engine.ThrowError( exc.Message );
            engine.ResumeError();
            Died();
        }
    }

    /// <summary>
    /// Get the value of a setting of the current instance.
    /// </summary>
    /// <param name="name">The name of the setting.</param>
    /// <returns>The value for the setting, or -1 if the setting is not known.</returns>
    public virtual int GetSetting( string name )
    {

    }

    /// <summary>
    /// Find a library.
    /// </summary>
    /// <param name="library">The library name to find.</param>
    /// <param name="version">The version the library should have.</param>
    /// <returns>The library if found, <see langword="null"/> otherwise.</returns>
    public virtual ScriptInfo FindLibrary( string library, int version )
    {

    }

    /// <summary>
    /// A script in multiplayer waits for the server to handle its DoCommand.<br/>
    /// It keeps waiting for this until this function is called.
    /// </summary>
    public void Continue()
    {
        Debug.Assert( suspend < 0 );
        suspend--;
    }

    /// <summary>
    /// Run the GameLoop of a script.
    /// </summary>
    public void GameLoop()
    {
        ScriptObject.ActiveInstance active = new ScriptObject.ActiveInstance( this );

        if ( IsDead() )
        {
            return;
        }

        if ( engine.HasScriptCrashed() )
        {
            // The script crashed during saving, kill it here
            Died();
            return;
        }

        if ( isPaused )
        {
            return;
        }

        controller.ticks++;

        if ( suspend < -1 )
        {
            suspend++; // Multiplayer suspend, increase up to -1
        }

        if ( suspend < 0 )
        {
            return; // Multiplayer suspend, wait for Continue()
        }

        if ( --suspend > 0 )
        {
            return; // Singleplayer suspend, decrease to 0
        }

        currentCompany = ScriptObject.GetCompany();

        // If there is a callback to call, call that first
        if ( callback != null )
        {
            if ( isSaveDataOnStack )
            {
                throw new NotImplementedException( "sq_poptop(this->engine->GetVM())" );
                isSaveDataOnStack = false;
            }

            try
            {
                callback( this );
            }
            catch ( Exception exc )
            {
                if ( exc is ScriptSuspendException scriptExc )
                {
                    suspend = scriptExc.GetSuspendTime();
                    callback = scriptExc.GetSuspendCallback();
                }

                return;
            }
        }

        suspend = 0;
        callback = null;

        if ( !isStarted )
        {
            try
            {
                ScriptObject.SetAllowDoCommand( false );

                // Run the constructor if it exists, don't allow any DoCommands in it
                if ( engine.MethodExists( instance, "constructor" ) )
                {
                    if ( !engine.CallMethod( instance, "constructor", MAX_CONSTRUCTOR_OPS ) || engine.IsSuspended() )
                    {
                        if ( engine.IsSuspended() )
                        {
                            ScriptLog.Error( "This script took too long to initialize. Script is not started." );
                        }

                        Died();
                        return;
                    }
                }

                if ( !CallLoad() || engine.IsSuspended() )
                {
                    if ( engine.IsSuspended() )
                    {
                        ScriptLog.Error( "This script took too long in the Load function. Script is not started." );
                    }

                    Died();
                    return;
                }

                ScriptObject.SetAllowDoCommand( true );

                // Start the script by calling Start()
                if ( !engine.CallMethod( instance, "start", settingsGame.script.scriptMaxOpcodeTillSuspend ) || !engine.IsSuspended() )
                {
                    Died();
                }
            }
            catch ( Exception exc )
            {
                if ( exc is ScriptSuspendException suspendExc )
                {
                    suspend = suspendExc.GetSuspendTime();
                    callback = suspendExc.GetSuspendCallback();
                }

                if ( exc is ScriptFatalErrorException fatalExc )
                {
                    isDead = true;
                    engine.ThrowError( fatalExc.GetErrorMessage() );
                    engine.ResumeError();
                    Died();
                }
            }

            isStarted = true;
            return;
        }

        if ( isSaveDataOnStack )
        {
            throw new NotImplementedException( "sq_poptop(this->engine->GetVM())" );
            isSaveDataOnStack = false;
        }

        // Continue the VM
        try
        {
            if ( !engine.Resume( settingsGame.script.scriptMaxOpcodeTillSuspend ) )
            {
                Died();
            }
        }
        catch ( Exception exc )
        {
            if ( exc is ScriptSuspendException suspendExc )
            {
                suspend = suspendExc.GetSuspendTime();
                callback = suspendExc.GetSuspendCallback();
            }

            if ( exc is ScriptFatalErrorException fatalExc )
            {
                isDead = true;
                engine.ThrowError(fatalExc.GetErrorMessage() );
                engine.ResumeError();
                Died();
            }
        }
    }

    /// <summary>
    /// Let the VM collect any garbage.
    /// </summary>
    public void CollectGarbage()
    {
        if ( isStarted && !IsDead() )
        {
            ScriptObject.ActiveInstance active = new ScriptObject.ActiveInstance( this );
            engine.CollectGarbage();
        }
    }

    /// <summary>
    /// Get the storage of this script.
    /// </summary>
    public ScriptStorage GetStorage()
    {
        return storage;
    }

    /// <summary>
    /// Get the log pointer of this script.
    /// </summary>
    /// <returns></returns>
    public ScriptLogTypes.LogData GetLogData()
    {
        ScriptObject.ActiveInstance active = new ScriptObject.ActiveInstance( this );

        return ScriptObject.GetLogData();
    }

    /// <summary>
    /// Return <see langword="true"/> / <see langword="false"/> reply for a DoCommand.
    /// </summary>
    public static void DoCommandReturn( ScriptInstance instance )
    {
        instance.engine.InsertResult( ScriptObject.GetLastCommandRes() );
    }

    /// <summary>
    /// Return a VehicleID reply for a DoCommand.
    /// </summary>
    public static void DoCommandReturnVehicleID( ScriptInstance instance )
    {
        instance.engine.InsertResult( EndianBufferReader.ToValue<VehicleID>( ScriptObject.GetLastCommandResData() ) );
    }

    /// <summary>
    /// Return a SignID reply for a DoCommand.
    /// </summary>
    public static void DoCommandReturnSignID( ScriptInstance instance )
    {
        instance.engine.InsertResult(EndianBufferReader.ToValue<SignID>(ScriptObject.GetLastCommandResData() ) );
    }

    /// <summary>
    /// Return a GroupID reply for a DoCommand.
    /// </summary>
    public static void DoCommandReturnGroupID( ScriptInstance instance )
    {
        instance.engine.InsertResult( EndianBufferReader.ToValue<GroupID>( ScriptObject.GetLastCommandResData() ) );
    }

    /// <summary>
    /// Return a GoalID reply for a DoCommand.
    /// </summary>
    public static void DoCommandReturnGoalID( ScriptInstance instance )
    {
        instance.engine.InsertResult( EndianBufferReader.ToValue<GoalID>( ScriptObject.GetLastCommandResData() ) );
    }

    /// <summary>
    /// Return a StoryPageID reply for a DoCommand.
    /// </summary>
    public static void DoCommandReturnStoryPageID( ScriptInstance instance )
    {
        instance.engine.InsertResult( EndianBufferReader.ToValue<StoryPageID>( ScriptObject.GetLastCommandResData() ) );
    }

    /// <summary>
    /// Return a StoryPageElementID reply for a DoCommand.
    /// </summary>
    public static void DoCommandReturnStoryPageElementID( ScriptInstance instance )
    {
        instance.engine.InsertResult( EndianBufferReader.ToValue<StoryPageElementID>( ScriptObject.GetLastCommandResData() ) );
    }

    /// <summary>
    /// Return a LeagueTableID reply for a DoCommand.
    /// </summary>
    public static void DoCommandReturnLeagueTableID( ScriptInstance instance )
    {
        instance.engine.InsertResult( EndianBufferReader.ToValue<LeagueTableID>( ScriptObject.GetLastCommandResData() ) );
    }

    /// <summary>
    /// Get the controller attached to the instance.
    /// </summary>
    public ScriptController GetController()
    {
        return controller;
    }

    /// <summary>
    /// Return the "this script died" value.
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }

    /// <summary>
    /// Return whether the script is alive.
    /// </summary>
    public bool IsAlive()
    {
        return !IsDead() && !inShutdown;
    }

    /// <summary>
    /// Call the script Save function and save all data in the savegame.
    /// </summary>
    public void Save()
    {
        ScriptObject.ActiveInstance active = new ScriptObject.ActiveInstance( this );

        // Don't save data if the script didn't start yet or if it crashed
        if ( engine == null || engine.HasScriptCrashed() )
        {
            SaveEmpty();
            return;
        }

        SQVM vm = engine.GetVM();

        if ( isSaveDataOnStack )
        {
            Script.scriptS1Byte = 1;
            S1Object( null, Script.scriptByte );

            // Save the data that was just loaded
            SaveObject( vm, -1, Script.SQUIRREL_MAX_DEPTH, false );
        }
        else if ( !isStarted )
        {
            SaveEmpty();
            return;
        }
        else if ( engine.MethodExists( instance, "Save" ) )
        {
            SQObject savedata;

            // We don't want to be interrupted during the save function
            bool backupAllow = ScriptObject.GetAllowDoCommand();
            ScriptObject.SetAllowDoCommand( false );

            try
            {
                if ( !engine.CallMethod( instance, "Save", savedata, MAX_SL_OPS ) )
                {
                    // The script crashed in the Save function, we can't kill
                    // it here, but do so in the next script tick
                    SaveEmpty();
                    engine.CrashOccurred();
                    return;
                }
            }
            catch ( Exception exc )
            {
                if ( exc is ScriptFatalErrorException fatalExc )
                {
                    // If we don't mark the script as dead here cleaning up the squirrel
                    // stack could throw ScriptFatalErrorException again
                    isDead = true;
                    engine.ThrowError( fatalExc.GetErrorMessage() );
                    engine.ResumeError();
                    SaveEmpty();

                    // We can't kill the script here, so mark it as crashed (not dead) and
                    // kill it in the next script tick
                    isDead = false;
                    engine.CrashOccurred();
                    return;
                }
            }

            ScriptObject.SetAllowDoCommand( backupAllow );

            if ( !SQ_IsTable( savedata ) )
            {
                ScriptLog.Error( engine.IsSuspended() ? "This script took too long to Save." : "Save function should return a table." );
                SaveEmpty();
                engine.CrashOccurred();
                return;
            }

            SQ_PushObject( vm, savedata );

            if ( SaveObject( vm, -1, Script.SQUIRREL_MAX_DEPTH, true ) )
            {
                Script.scriptS1Byte = 1;
                S1Object( null, Script.scriptByte );
                SaveObject( vm, -1, Script.SQUIRREL_MAX_DEPTH, false );
                isSaveDataOnStack = true;
            }
            else
            {
                SaveEmpty();
                engine.CrashOccurred();
            }
        }
        else
        {
            ScriptLog.Warning( "Save function is not implemented." );
            Script.scriptS1Byte = 0;
            S1Object( null, Script.scriptByte );
        }
    }

    /// <summary>
    /// Don't save any data in the savegame.
    /// </summary>
    public static void SaveEmpty()
    {
        Script.scriptS1Byte = 0;
        S1Object( null, Script.scriptByte );
    }

    /// <summary>
    /// Load data from a savegame.
    /// </summary>
    /// <param name="version">The version of the script when saving, or -1 if this was not the original script saving the game.</param>
    /// <returns>A pointer to loaded data.</returns>
    public static ScriptData Load( int version )
    {

    }

    /// <summary>
    /// Store loaded data on the stack.
    /// </summary>
    /// <param name="data">The loaded data to store on the stack.</param>
    public void LoadOnStack( ScriptData data )
    {

    }

    /// <summary>
    /// Load and discard data from a savegame.
    /// </summary>
    public static void LoadEmpty()
    {

    }

    /// <summary>
    /// Suspends the script for the current tick and then pause the execution<br/>
    /// of script. The script will not be resumed from its suspended state<br/>
    /// until the script has been unpaused.
    /// </summary>
    public void Pause()
    {
        // Suspend script
        SQVM vm = engine.GetVM();
        Squirrel.DecreaseOps( vm, settingsGame.script.scriptMaxOpcodeTillSuspend );

        isPaused = true;
    }

    /// <summary>
    /// Checks if the script is paused.
    /// </summary>
    /// <returns><see langword="true"/> if the script is paused, otherwise <see langword="false"/>.</returns>
    public bool IsPaused()
    {
        return isPaused;
    }

    /// <summary>
    /// Resume execution of the script. This function will not actually execute<br/>
    /// the script, but set a flag that the script is executed by the usual<br/>
    /// mechanism that executes the script.
    /// </summary>
    public void Unpause()
    {
        isPaused = false;
    }

    /// <summary>
    /// Get the number of operations the script can execute before being suspended.<br/>
    /// This function is safe to call from within a function called by the script.
    /// </summary>
    /// <returns>The number of operations to execute.</returns>
    public SQInteger GetOpsTillSuspend()
    {

    }

    /// <summary>
    /// DoCommand callback function for all commands executed by scripts.
    /// </summary>
    /// <param name="result">The result of the command.</param>
    /// <param name="data">Command data as given to DoCommandPInternal.</param>
    /// <param name="resultData">Extra data return from the command.</param>
    /// <param name="cmd">Cmd as given to DoCommandPInternal.</param>
    /// <returns><see langword="true"/> if we handled the result.</returns>
    public bool DoCommandCallback( CommandCost result, CommandDataBuffer data, CommandDataBuffer resultData, Commands cmd )
    {

    }

    /// <summary>
    /// Insert an event for this script.
    /// </summary>
    /// <param name="scriptEvent">The event to insert.</param>
    public void InsertEvent( ScriptEvent scriptEvent )
    {

    }

    /// <summary>
    /// Check if the instance is sleeping, which either happened because the<br/>
    /// script executed a DoCommand, executed <see cref="Sleep"/> or it has been<br/>
    /// paused.
    /// </summary>
    public bool IsSleeping()
    {
        return suspend != 0;
    }

    public ulong GetAllocatedMemory()
    {

    }

    /// <summary>
    /// Indicates whether this instance is currently being destroyed.
    /// </summary>
    public bool InShutdown()
    {
        return inShutdown;
    }

    /// <summary>
    /// Decrease the ref count of a squirrel object.
    /// </summary>
    /// <param name="obj">The object to release.</param>
    public void ReleaseSQObject( SQObject obj )
    {

    }

    /// <summary>
    /// Register all API functions to the VM.
    /// </summary>
    protected virtual void RegisterAPI()
    {
        throw new NotImplementedException( "squirrel_register_std(this->engine)" );
    }

    /// <summary>
    /// Load squirrel scripts to emulate an older API.
    /// </summary>
    /// <param name="dir">Subdirectory to find the scripts in.</param>
    /// <param name="apiVersions">List of available versions of the script type.</param>
    /// <returns><see langword="true"/> if script loading should proceed.</returns>
    protected bool LoadCompatibilityScripts( Subdirectory dir, Span<string> apiVersions )
    {
        // Don't try to load compatibility scripts for the current version
        if ( versionAPI == apiVersions[0] )
        {
            return true;
        }

        ScriptLog.Info( $"Downgrading API to be compatible with {versionAPI}" );

        // Downgrade the API till we are the same version as the script, the last
        // entry in the list is always the current version, so skip that one
        for ( int itIdx = 0; itIdx != apiVersions.Length; itIdx++ )
        {
            string it = apiVersions[itIdx];

            if ( LoadCompatibilityScript( it, dir ) )
            {
                return false;
            }

            if ( it == versionAPI )
            {
                break;
            }
        }

        return true;
    }

    /// <summary>
    /// Tell the script it died.
    /// </summary>
    protected virtual void Died()
    {
        Console.WriteLine( "The script died unexpectedly." );
        isDead = true;
        inShutdown = true;

        lastAllocatedMemory = GetAllocatedMemory();

        if ( instance != null )
        {
            engine.ReleaseObject( instance );
        }

        instance = null;
        engine = null;
    }

    /// <summary>
    /// Get the callback handling DoCommands in case of networking.
    /// </summary>
    protected virtual CommandCallbackData GetDoCommandCallbackData()
    {

    }

    /// <summary>
    /// Load the dummy script.
    /// </summary>
    protected virtual void LoadDummyScript()
    {

    }

    /// <summary>
    /// Call the script Load function if it exists and data was loaded<br/>
    /// from a savegame.
    /// </summary>
    private bool CallLoad()
    {

    }

    /// <summary>
    /// Load squirrel script for a specific version to emulate an older API.
    /// </summary>
    /// <param name="apiVersion">API version to load scripts for.</param>
    /// <param name="dir">Subdirectory to find the scripts in.</param>
    /// <returns><see langword="true"/> if script loading should proceed.</returns>
    private bool LoadCompatibilityScript( string apiVersion, Subdirectory dir )
    {

    }

    /// <summary>
    /// Save one object (int / string / array / table) to the savegame.
    /// </summary>
    /// <param name="vm">The virtual machine to get all the data from.</param>
    /// <param name="index">The index on the squirrel stack of the elements to save.</param>
    /// <param name="maxDepth">The maximum depth of recursive arrays / tables will be stored with before an error is returned.</param>
    /// <param name="test">If <see langword="true"/>, don't really store the data but only check if it is valid.</param>
    /// <returns><see langword="true"/> if the saving was successful.</returns>
    private static bool SaveObject( SQVM vm, SQInteger index, int maxDepth, bool test )
    {
        if ( maxDepth == 0 )
        {
            ScriptLog.Error( "Savedata can only be nested to 25 deep. No data saved." ); // SQUIRREL_MAX_DEPTH = 25
            return false;
        }

        throw new NotImplementedException( "switch (sq_gettype(vm, index))" );
        //switch ()
    }

    /// <summary>
    /// Load all objects from a savegame.
    /// </summary>
    /// <returns><see langword="true"/> if the loading was successful.</returns>
    private static bool LoadObjects( ScriptData data )
    {
        S1Object( null, Script.scriptByte );

        switch ( Script.scriptS1Byte )
        {
            case SQSL_INT:
                long value = 0;
                S1Copy( ref value, 1, IsSaveGameVersionBefore( SLV_SCRIPT_LONG ) ? SLE_FILE_I | SLE_VAR_L : SLE_LONG );
                
                if ( data != null )
                {
                    data.Add( (SQInteger)value );
                }

                return true;

            case SQSL_STRING:
                S1Object( null, Script.scriptByte );
                string buf = string.Empty;
                S1Copy( buf, Script.scriptS1Byte, SLE_CHAR );

                if ( data != null )
                {
                    data.Add( StrMakeValid( buf ) );
                }

                return true;

            case SQSL_ARRAY:
            case SQSL_TABLE:
                if ( data != null )
                {
                    data.Add( (SQSaveLoadType)Script.scriptS1Byte );
                }

        }
    }

    /// <inheritdoc cref="LoadObjects(ScriptData)"/>
    private static bool LoadObjects( SQVM vm, ScriptData data )
    {

    }
}