namespace OpenTTD.Script;

/// <summary>
/// The type of script we're working with, i.e. for who is it?
/// </summary>
public enum ScriptType : byte
{
    AI, // The script is for AI scripts
    GS, // The script is for Game scripts
}

public class Squirrel
{
    private delegate void SQPrintFunc( bool errorMsg, string message );

    private SquirrelVM vm; // The VirtualMachine instance for squirrel
    private object globalPointer; // Can be set by whoever initializes squirrel
    private SQPrintFunc printFunc; // Points to either null, or a custom print handler
    private bool crashed; // True if the squirrel script made an error
    private int overdrawnOps; // The amount of operations we have overdrawn
    private string APIName; // Name of the API used for this squirrel
    private ScriptAllocator allocator; // Allocator object used by this script

    public Squirrel( string APIName )
    {

    }

    ~Squirrel()
    {

    }

    /// <summary>
    /// The internal RunError handler. It looks up the real error and calls <see cref="RunError"/> with it.
    /// </summary>
    private static SQInteger _RunError( SquirrelVM vm )
    {

    }

    /// <summary>
    /// Get the API name.
    /// </summary>
    private string GetName()
    {
        return APIName;
    }

    /// <summary>
    /// Perform all initialization steps to create the engine.
    /// </summary>
    private void Initialize()
    {

    }

    /// <summary>
    /// Perform all cleanups for the engine.
    /// </summary>
    private void Uninitialize()
    {

    }

    /// <summary>
    /// The CompileError handler.
    /// </summary>
    protected static void CompileError( SquirrelVM vm, string desc, string source, SQInteger line, SQInteger column )
    {

    }

    /// <summary>
    /// The RunError handler.
    /// </summary>
    protected static void RunError( SquirrelVM vm, SQString error )
    {

    }

    /// <summary>
    /// If a user runs 'print' inside a script, this function gets the params.
    /// </summary>
    protected static void PrintFunc( SquirrelVM vm, string s )
    {

    }

    /// <summary>
    /// If an error has to be print, this function is called.
    /// </summary>
    protected static void ErrorPrintFunc( SquirrelVM vm, string s )
    {

    }

    /// <summary>
    /// Get the squirrel VM. Try to avoid doing this.
    /// </summary>
    public SquirrelVM GetVM()
    {
        return vm;
    }

    /// <summary>
    /// Load a script.
    /// </summary>
    /// <param name="script">The full script-name to load.</param>
    /// <returns><see langword="false"/> if loading failed.</returns>
    public bool LoadScript( string script )
    {

    }

    public bool LoadScript( SquirrelVM vm, string script, bool inRoot = true )
    {

    }

    /// <summary>
    /// Load a file to a given VM.
    /// </summary>
    public SQResult LoadFile( SquirrelVM vm, string filename, SQBool printError )
    {

    }


}