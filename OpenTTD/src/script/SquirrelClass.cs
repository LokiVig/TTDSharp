using System;

namespace OpenTTD.Script;

/// <summary>
/// The template to define classes in Squirrel. It takes care of the creation<br/>
/// and calling of such classes, to minimize the API layer.
/// </summary>
/// <typeparam name="CL">The class to define.</typeparam>
/// <typeparam name="ST">The <see cref="ScriptType"/> of the class to define.</typeparam>
public class DefSQClass<CL, ST>
    where CL : class
    where ST : unmanaged, Enum
{
    private string classname;

    public DefSQClass( string classname )
    {
        this.classname = classname;
    }

    /// <summary>
    /// This defines a method inside a class for Squirrel.
    /// </summary>
    public void DefSQMethod<Func>( Squirrel engine, Func functionProc, string functionName )
    {
        unsafe
        {
            engine.AddMethod( functionName, Script.DefSQNonStaticCallback<CL, Func, ST>, 0, null, functionProc, sizeof( Func ) );
        }
    }

    /// <summary>
    /// This defines a method inside a class for Squirrel, which has access to the 'engine' (experts only!).
    /// </summary>
    public void DefSQAdvancedMethod<Func>( Squirrel engine, Func functionProc, string functionName )
    {
        unsafe
        {
            engine.AddMethod( functionName, Script.DefSQAdvancedNonStaticCallback<CL, Func, ST>, 0, null, functionProc, sizeof( Func ) );
        }
    }

    /// <summary>
    /// This defines a method inside a class for Squirrel with defined params.<br/>
    /// <b>NOTE:</b> If you define <paramref name="nParam"/>, make sure that the first param is always 'x',<br/>
    /// which is the '<see langword="this"/>' inside the function. This is hidden from the rest<br/>
    /// of the code, but without it calling your function will fail!
    /// </summary>
    public void DefSQMethod<Func>( Squirrel engine, Func functionProc, string functionName, int nParam, string sParam )
    {
        unsafe
        {
            engine.AddMethod( functionName, Script.DefSQNonStaticCallback<CL, Func, ST>, nParam, sParam, functionProc, sizeof( Func ) );
        }
    }

    /// <summary>
    /// This defines a static method inside a class for Squirrel, which has access to the 'engine' (experts only!).
    /// </summary>
    public void DefSQStaticMethod<Func>( Squirrel engine, Func functionProc, string functionName )
    {
        unsafe
        {
            engine.AddMethod( functionName, Script.DefSQStaticCallback<CL, Func>, 0, null, functionProc, sizeof( Func ) );
        }
    }

    /// <summary>
    /// This defines a static method inside a class for Squirrel, which has access to the 'engine' (experts only!).
    /// </summary>
    public void DefSQAdvancedStaticMethod<Func>( Squirrel engine, Func functionProc, string functionName )
    {
        unsafe
        {
            engine.AddMethod( functionName, Script.DefSQAdvancedStaticCallback<CL, Func>, 0, null, functionProc, sizeof( Func ) );
        }
    }

    /// <summary>
    /// This defines a static method inside a class for Squirrel with defined params.<br/>
    /// <b>NOTE:</b> If you define <paramref name="nParam"/>, make sure that the first param is always 'x',<br/>
    /// which is the '<see langword="this"/>' inside the function. This is hidden from the rest<br/>
    /// of the code, but without it calling your function will fail!
    /// </summary>
    public void DefSQStaticMethod<Func>( Squirrel engine, Func functionProc, string functionName, int nParam, string sParam )
    {
        unsafe
        {
            engine.AddMethod( functionName, Script.DefSQStaticCallback<CL, Func>, nParam, sParam, functionProc, sizeof( Func ) );
        }
    }

    public void DefSQConst<Var>( Squirrel engine, Var value, string varName )
    {
        engine.AddConst( varName, value );
    }

    public void PreRegister( Squirrel engine )
    {
        engine.AddClassBegin( classname );
    }

    public void PreRegister( Squirrel engine, string parentClass )
    {
        engine.AddClassBegin( classname, parentClass );
    }

    public void AddConstructor<Func>( Squirrel engine, int nParams, string sParams )
    {
        engine.AddMethod( "constructor", Script.DefSQConstructorCallback < CL, Func, typeof( int ) >, nParams, sParams );
    }

    public void AddSQAdvancedConstructor( Squirrel engine )
    {
        engine.AddMethod( "constructor", Script.DefSQAdvancedConstructorCallback<CL>, 0, null );
    }

    public void PostRegister( Squirrel engine )
    {
        engine.AddClassEnd();
    }
}