namespace OpenTTD.Script.API;

public static partial class ScriptAPI
{
    /// <summary>
    /// The callback function for Mode-classes.
    /// </summary>
    public delegate bool ScriptModeProc();

    /// <summary>
    /// The callback function for Async Mode-classes.
    /// </summary>
    public delegate bool ScriptAsyncModeProc();
}

/// <summary>
/// Simple counted object. Use it as a base of your <see langword="struct"/> / <see langword="class"/> if you want to use<br/>
/// basic reference counting. Your <see langword="struct"/> / <see langword="class"/> will destroy and free itself when<br/>
/// the last reference to it is released (using <see cref="Release"/>). The initial reference<br/>
/// count (when it is created) is zero (don't forget to <see cref="AddRef"/> at least once if not using<br/>
/// <see cref="ScriptObjectRef"/>.
/// </summary>
public class SimpleCountedObject
{
    private int refCount;

    public SimpleCountedObject()
    {
        refCount = 0;
    }

    public void AddRef()
    {
        refCount++;
    }

    public void Release()
    {

    }

    public virtual void FinalRelease()
    {

    }
}

/// <summary>
/// User-parent object of all API classes. You should never use this class in<br/>
/// your script, as it doesn't publish any public functions. It is used<br/>
/// internally to have a a common place to handle general things, like internal<br/>
/// command processing, and command-validation checks.
/// </summary>
public class ScriptObject : SimpleCountedObject
{
    /// <summary>
    /// A class that handles the current active instance. By instantiating it at<br/>
    /// the beginning of a function with the current active instance, it remains<br/>
    /// active till the scope of the variable closes. It then automatically<br/>
    /// reverts to the active instance it was before instantiating.
    /// </summary>
    protected class ActiveInstance
    {
        private static ScriptInstance active; // The global currently active instance

        private ScriptInstance lastActive; // The active instance before we got instantiated
        private ScriptAllocatorScope alcScope; // Keep the correct allocator for the script instance activated

        public ActiveInstance( ScriptInstance instance )
        {

        }
    }

    /// <summary>
    /// Save this object.<br/>
    /// Must push 2 elements on the stack:<br/>
    /// - The name (classname without "Script") of the object (OT_STRING)<br/>
    /// - The data for the object (any supported types)
    /// </summary>
    /// <returns><see langword="true"/> if saving this type is supported.</returns>
    protected virtual bool SaveObject( SQVM vm )
    {
        return false;
    }

    /// <summary>
    /// Load this object.<br/>
    /// The data for the object must be pushed on the stack before the call.
    /// </summary>
    /// <returns><see langword="true"/> if loading this type is supported.</returns>
    protected virtual bool LoadObject( SQVM vm )
    {
        return false;
    }

    /// <summary>
    /// Store the latest result of a <see cref="DoCommand"/> per company.
    /// </summary>
    /// <param name="res">The result of the last command.</param>
    public static void SetLastCommandRes( bool res )
    {

    }

    /// <summary>
    /// Store the extra data return by the last <see cref="DoCommand"/>.
    /// </summary>
    /// <param name="data">Extra data return by the command.</param>
    public static void SetLastCommandData( CommandDataBuffer data )
    {

    }

    /// <summary>
    /// Get the currently active instance.
    /// </summary>
    /// <returns>The instance.</returns>
    public static ScriptInstance GetActiveInstance()
    {

    }

    /// <summary>
    /// Get a reference of the randomizer that brings this script random values.
    /// </summary>
    /// <param name="owner">The owner / script to get the randomizer for. This defaults to <see cref="ScriptObject.GetRootCompany"/>.</param>
    public static Randomizer GetRandomizer( Owner owner = ScriptObject.GetRootCompany() )
    {

    }

    /// <summary>
    /// Initialize / reset the script random states. The state of the scripts are<br/>
    /// based on the current <see cref="random"/> seed, but <see cref="random"/> does not get changed.
    /// </summary>
    public static void InitializeRandomizers()
    {

    }

    /// <summary>
    /// Templated wrapper that exposes the command parameter arguments<br/>
    /// on the various <see cref="DoCommand"/> calls.
    /// </summary>
    /// <typeparam name="TCmd">The command ID to execute.</typeparam>
    /// <typeparam name="TRet">Return type of the command.</typeparam>
    protected struct ScriptDoCommandHelper<TCmd, TRet>
        where TCmd : Commands
    {
        public static bool Do( ScriptSuspendCallbackProc callback, params object[] args )
        {
            return Execute( callback, args );
        }

        public static bool Do( params object[] args )
        {
            return Execute( null, args );
        }

        private static bool Execute( ScriptSuspendCallbackProc callback, params object[] args )
        {
            Tuple<bool /* err */, bool /* estimateOnly */, bool /* asynchronous */, bool /* networking */> prep = DoCommandPrep();
            
            if ( prep.Item1 ) // err
            {
                return false;
            }

            if ( ( GetCommandFlags<TCmd>() & CommandFlag.StrCtrl ) == 0 )
            {
                //ScriptObjectInternal.SanitizeSingleStringHelper(args,)
            }

            TileIndex tile = new TileIndex();

            if ( args[0] is TileIndex tileArg )
            {
                tile = tileArg;
            }

            // Do not even think about executing out-of-bounds tile-commands
            if ( tile != 0 && ( tile >= Map.Size() || ( !IsValidTile( tile ) && ( GetCommandFlags<TCmd>() & CommandFlag.AllTiles ) == 0 ) ) )
            {
                return false;
            }

            // Store the command for command callback validation
            if ( !prep.Item2 && prep.Item4 ) // !estimateOnly && networking
            {
                //ScriptObject.SetLastCommand( EndianBufferWriter<CommandDataBuffer>.FromValue( args ),  );
            }

            // TODO: Continue working on this!
        }
    }

    /// <summary>
    /// Store the latest command executed by the script.
    /// </summary>
    protected static void SetLastCommand( CommandDataBuffer data, Commands cmd )
    {

    }

    /// <summary>
    /// Check if it's the latest command executed by the script.
    /// </summary>
    protected static bool CheckLastCommand( CommandDataBuffer data, Commands cmd )
    {

    }

    /// <summary>
    /// Sets the <see cref="DoCommand"/> costs counter to <paramref name="value"/>.
    /// </summary>
    protected static void SetDoCommandCosts( Money value )
    {

    }

    /// <summary>
    /// Increase the current value of the <see cref="DoCommand"/> costs counter.
    /// </summary>
    protected static void IncreaseDoCommandCosts( Money value )
    {

    }

    /// <summary>
    /// Get the current <see cref="DoCommand"/> costs counter.
    /// </summary>
    protected static Money GetCommandCosts()
    {

    }

    /// <summary>
    /// Set the <see cref="DoCommand"/> last error.
    /// </summary>
    protected static void SetLastError( ScriptErrorType lastError )
    {

    }

    /// <summary>
    /// Get the <see cref="DoCommand"/> last error.
    /// </summary>
    protected static ScriptErrorType GetLastError()
    {

    }

    /// <summary>
    /// Set the road type.
    /// </summary>
    protected static void SetRoadType( RoadType roadType )
    {

    }

    /// <summary>
    /// Get the road type.
    /// </summary>
    protected static RoadType GetRoadType()
    {

    }

    /// <summary>
    /// Set the rail type.
    /// </summary>
    protected static void SetRailType( RailType railType )
    {

    }

    /// <summary>
    /// Get the rail type.
    /// </summary>
    protected static RailType GetRailType()
    {

    }

    /// <summary>
    /// Set the current mode of your script to this proc.
    /// </summary>
    protected static void SetDoCommandMode( ScriptAPI.ScriptModeProc proc, ScriptObject instance )
    {

    }

    /// <summary>
    /// Get the current mode your script is currently under.
    /// </summary>
    protected static ScriptAPI.ScriptModeProc GetDoCommandMode()
    {

    }

    /// <summary>
    /// Get the instance of the current mode your script is currently under.
    /// </summary>
    protected static ScriptObject GetDoCommandModeInstance()
    {

    }

    /// <summary>
    /// Set the current async mode of your script to this proc.
    /// </summary>
    protected static void SetDoCommandAsyncMode( ScriptAPI.ScriptAsyncModeProc proc, ScriptObject instance )
    {

    }

    /// <summary>
    /// Get the current async mode your script is currently under.
    /// </summary>
    protected static ScriptAPI.ScriptAsyncModeProc GetDoCommandAsyncMode()
    {

    }

    /// <summary>
    /// Get the instance of the current async mode your script is currently under.
    /// </summary>
    protected static ScriptObject GetDoCommandAsyncModeInstance()
    {

    }

    /// <summary>
    /// Set the delay of the <see cref="DoCommand"/>.
    /// </summary>
    protected static void SetDoCommandDelay( uint ticks )
    {

    }

    /// <summary>
    /// Get the delay of the <see cref="DoCommand"/>.
    /// </summary>
    protected static uint GetDoCommandDelay()
    {

    }

    /// <summary>
    /// Get the latest result of a <see cref="DoCommand"/>.
    /// </summary>
    protected static bool GetLastCommandRes()
    {

    }

    /// <summary>
    /// Get the extra return data from the last <see cref="DoCommand"/>.
    /// </summary>
    protected static CommandDataBuffer GetLastCommandResData()
    {

    }

    /// <summary>
    /// Store a <see cref="allowDoCommand"/> per company.
    /// </summary>
    /// <param name="allow">The new allow.</param>
    protected static void SetAllowDoCommand( bool allow )
    {

    }

    /// <summary>
    /// Get the internal value of <see cref="allowDoCommand"/>. This can differ<br/>
    /// from <see cref="CanSuspend"/> if the reason we are not allowed<br/>
    /// to execute a <see cref="DoCommand"/> is in squirrel and not the API.<br/>
    /// In that case use this function to restore the previous value.
    /// </summary>
    /// <returns><see langword="true"/> if <see cref="DoCommand"/>s are allowed in the current scope.</returns>
    protected static bool GetAllowDoCommand()
    {

    }

    /// <summary>
    /// Set the current company to execute commands for or request<br/>
    /// information about.
    /// </summary>
    /// <param name="company">The new company.</param>
    protected static void SetCompany( CompanyID company )
    {

    }

    /// <summary>
    /// Get the current company we are executing commands for or<br/>
    /// requesting information about.
    /// </summary>
    /// <returns>The current company.</returns>
    protected static CompanyID GetCompany()
    {

    }

    /// <summary>
    /// Get the root company, the company that the script really<br/>
    /// runs under / for.
    /// </summary>
    /// <returns>The root company.</returns>
    protected static CompanyID GetRootCompany()
    {

    }

    /// <summary>
    /// Set the cost of the last command.
    /// </summary>
    protected static void SetLastCost( Money lastCost )
    {

    }

    /// <summary>
    /// Get the cost of the last command.
    /// </summary>
    protected static Money GetLastCost()
    {

    }

    /// <summary>
    /// Set a variable that can be used by callback functions to pass information.
    /// </summary>
    protected static void SetCallbackVariable( int index, int value )
    {

    }

    /// <summary>
    /// Get the variable that is used by callback functions to pass information.
    /// </summary>
    protected static int GetCallbackVariable( int index )
    {

    }

    /// <summary>
    /// Can we suspend the script at this moment?
    /// </summary>
    protected static bool CanSuspend()
    {

    }

    /// <summary>
    /// Get the pointer to store event data in.
    /// </summary>
    protected static IntPtr GetEventPointer()
    {

    }

    /// <summary>
    /// Get the pointer to store event data in.
    /// </summary>
    protected static ScriptLogTypes.LogData GetLogData()
    {

    }

    private static Tuple<bool, bool, bool, bool> DoCommandPrep()
    {

    }

    private static bool DoCommandProcessResult( CommandCost res, Script.SuspendCallbackProc callback, bool estimateOnly, bool asynchronous )
    {

    }

    // TODO: using RandomizerArray = ReferenceThroughBaseContainer<std::array<Randomizer, OWNER_END.base()>>;
    // TODO: private static RandomizerArray randomStates;
}

public static class ScriptObjectInternal
{
    /// <summary>
    /// Validate a single string argument coming from network.
    /// </summary>
    public static void SanitizeSingleStringHelper<T>( T data )
        where T : class
    {
        if ( data is string )
        {
            // The string must be valid, i.e. not contain special codes, since some
            // can be made with GSText, make sure the control codes are removed
            data = String.MakeValid( data, SVS_NONE );
        }
    }

    // TODO: Continue this class!
}

/// <summary>
/// Internally used class to automate the <see cref="ScriptObject"/> reference counting.
/// </summary>
public class ScriptObjectRef<T>
    where T : ScriptObject
{
    private T data; // The reference counted object

    /// <summary>
    /// Create the reference counter for the given <see cref="ScriptObject"/> instance.
    /// </summary>
    /// <param name="data">The underlying object.</param>
    public ScriptObjectRef( T data )
    {
        this.data = data;

        if ( this.data != null )
        {
            data.AddRef();
        }
    }

    /// <summary>
    /// Move constructor.
    /// </summary>
    public ScriptObjectRef( ScriptObjectRef<T> other )
    {
        data = other.data;
        other.data = null;
    }

    /// <summary>
    /// Release the reference counted object.
    /// </summary>
    ~ScriptObjectRef()
    {
        if ( data != null )
        {
            data.Release();
        }
    }

    /// <summary>
    /// Dereferencing this reference returns a reference to the reference<br/>
    /// counted object.
    /// </summary>
    /// <param name="obj">Reference to the underlying object.</param>
    public static explicit operator T( ScriptObjectRef<T> obj )
    {
        return obj.data;
    }
}