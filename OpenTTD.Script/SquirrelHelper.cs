using System;

namespace OpenTTD.Script;

public static partial class Script
{
    public static SQInteger PushClassName<CL, ST>( SQVM vm )
        where CL : class
        where ST : unmanaged, Enum
    {

    }

    public struct Return<T>
    {
        public delegate int SetProc( SQVM vm, T res );

        public static SetProc Set;

        public Return( SetProc set )
        {
            Set = set;
        }
    }

    public static Return<byte> ReturnByte = new Return<byte>( ( SQVM vm, byte res ) => { SQ_PushInteger( vm, (int)res ); return 1; } );
    public static Return<ushort> ReturnUShort = new Return<ushort>( ( SQVM vm, ushort res ) => { SQ_PushInteger( vm, (int)res ); return 1; } );
    public static Return<uint> ReturnUInt = new Return<uint>( ( SQVM vm, uint res ) => { SQ_PushInteger( vm, (int)res ); return 1; } );
    public static Return<sbyte> ReturnSByte = new Return<sbyte>( ( SQVM vm, sbyte res ) => { SQ_PushInteger( vm, res ); return 1; } );
    public static Return<short> ReturnShort = new Return<short>( ( SQVM vm, short res ) => { SQ_PushInteger( vm, res ); return 1; } );
    public static Return<int> ReturnInt = new Return<int>( ( SQVM vm, int res ) => { SQ_PushInteger( vm, res ); return 1; } );
    public static Return<long> ReturnLong = new Return<long>( ( SQVM vm, long res ) => { SQ_PushInteger( vm, res ); return 1; } );
    public static Return<TileIndex> ReturnTileIndex = new Return<TileIndex>( ( SQVM vm, TileIndex res ) => { SQ_PushInteger( vm, (int)res.Base() ); return 1; } );
    public static Return<bool> ReturnBool = new Return<bool>( ( SQVM vm, bool res ) => { SQ_PushBool( vm, res ); return 1; } );
    //public static Return<string> ReturnString = new Return<string>((SQVM vm, string res) => { SQ_} );
    public static Return<SQObject> ReturnSQObject = new Return<SQObject>( ( SQVM vm, bool res ) => { SQ_PushObject( vm, res ); return 1; } );
    public static Return<Enum> ReturnEnum = new Return<Enum>( ( SQVM vm, Enum res ) => { SQ_PushInteger( vm, Convert.ToInt32( res ) ); return 1; } );
    public static Return<ConvertibleThroughBase> ReturnConvertibleThroughBase = new Return<ConvertibleThroughBase>( ( SQVM vm, ConvertibleThroughBase res ) => { SQ_PushInteger( vm, res.Base() ); return 1; } );
    public static Return<string> ReturnString = new Return<string>( ( SQVM vm, string res ) => { if ( !string.IsNullOrEmpty( res ) ) { SQ_PushString( vm, res, -1 ); } else { SQ_PushNull( vm ); } return 1; } );

    public struct Param<T>
    {
        public delegate T GetProc( SQVM vm, int index );

        public GetProc Get;

        public Param( GetProc get )
        {
            Get = get;
        }
    }

    public static Param<byte> ByteParam = new Param<byte>( ( SQVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<ushort> UShortParam = new Param<ushort>( ( SQVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<uint> UIntParam = new Param<uint>( ( SQVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<sbyte> SByteParam = new Param<sbyte>( ( SQVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<short> ShortParam = new Param<short>( ( SQVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<int> IntParam = new Param<int>( ( SQVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<long> LongParam = new Param<long>( ( SQVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<TileIndex> TileIndexParam = new Param<TileIndex>( ( SQVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<bool> BoolParam = new Param<bool>( ( SQVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<Enum> EnumParam = new Param<Enum>( ( SQVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<ConvertibleThroughBase> ConvertibleThroughBaseParam = new Param<ConvertibleThroughBase>( ( SQVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<string> StringParam = new Param<string>( ( SQVM vm, int index ) =>
    {
        // Convert whatever there is as parameter to a string
        SQ_ToString( vm, index );

        SQString tmp;
        SQ_GetString( vm, -1, out tmp );
        string result = StrMakeValid( tmp );
        SQ_PopTo( vm );
        return result;
    } );

    public static Param<Array> ArrayParam = new Param<Array>( ( SQVM vm, int index ) => { throw new NotImplementedException("...") } );

    public static SQInteger DefSQNonStaticCallback<TCls, TMethod, TType>( SQVM vm )
        where TCls : class
        where TType : unmanaged, Enum
    {
        // Find the amount of params we got
        int nParam = SQGetTop( vm );
        SQUserPointer ptr = null;
        SQUserPointer realInstance = null;
        SQObject instance;

        // Get the 'SQ' instance of this class
        Squirrel.GetInstance( vm, out instance );

        // Protect against calls to a non-static method in a static way
        SQPushRootTable( vm );
        PushClassName<TCls, TType>( vm );
    }


}