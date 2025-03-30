using System;

namespace OpenTTD.Script;

public static partial class Script
{
    public static SQInteger PushClassName<CL, ST>( SquirrelVM vm )
        where CL : class
        where ST : unmanaged, Enum
    {

    }

    public struct Return<T>
    {
        public delegate int SetProc( SquirrelVM vm, T res );

        public static SetProc Set;

        public Return( SetProc set )
        {
            Set = set;
        }
    }

    public static Return<byte> ReturnByte = new Return<byte>( ( SquirrelVM vm, byte res ) => { SQ_PushInteger( vm, (int)res ); return 1; } );
    public static Return<ushort> ReturnUShort = new Return<ushort>( ( SquirrelVM vm, ushort res ) => { SQ_PushInteger( vm, (int)res ); return 1; } );
    public static Return<uint> ReturnUInt = new Return<uint>( ( SquirrelVM vm, uint res ) => { SQ_PushInteger( vm, (int)res ); return 1; } );
    public static Return<sbyte> ReturnSByte = new Return<sbyte>( ( SquirrelVM vm, sbyte res ) => { SQ_PushInteger( vm, res ); return 1; } );
    public static Return<short> ReturnShort = new Return<short>( ( SquirrelVM vm, short res ) => { SQ_PushInteger( vm, res ); return 1; } );
    public static Return<int> ReturnInt = new Return<int>( ( SquirrelVM vm, int res ) => { SQ_PushInteger( vm, res ); return 1; } );
    public static Return<long> ReturnLong = new Return<long>( ( SquirrelVM vm, long res ) => { SQ_PushInteger( vm, res ); return 1; } );
    public static Return<TileIndex> ReturnTileIndex = new Return<TileIndex>( ( SquirrelVM vm, TileIndex res ) => { SQ_PushInteger( vm, (int)res.Base() ); return 1; } );
    public static Return<bool> ReturnBool = new Return<bool>( ( SquirrelVM vm, bool res ) => { SQ_PushBool( vm, res ); return 1; } );
    //public static Return<string> ReturnString = new Return<string>((SquirrelVM vm, string res) => { SQ_} );
    public static Return<SQObject> ReturnSQObject = new Return<SQObject>( ( SquirrelVM vm, bool res ) => { SQ_PushObject( vm, res ); return 1; } );
    public static Return<Enum> ReturnEnum = new Return<Enum>( ( SquirrelVM vm, Enum res ) => { SQ_PushInteger( vm, Convert.ToInt32( res ) ); return 1; } );
    public static Return<ConvertibleThroughBase> ReturnConvertibleThroughBase = new Return<ConvertibleThroughBase>( ( SquirrelVM vm, ConvertibleThroughBase res ) => { SQ_PushInteger( vm, res.Base() ); return 1; } );
    public static Return<string> ReturnString = new Return<string>( ( SquirrelVM vm, string res ) => { if ( !string.IsNullOrEmpty( res ) ) { SQ_PushString( vm, res, -1 ); } else { SQ_PushNull( vm ); } return 1; } );

    public struct Param<T>
    {
        public delegate T GetProc( SquirrelVM vm, int index );

        public GetProc Get;

        public Param( GetProc get )
        {
            Get = get;
        }
    }

    public static Param<byte> ByteParam = new Param<byte>( ( SquirrelVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<ushort> UShortParam = new Param<ushort>( ( SquirrelVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<uint> UIntParam = new Param<uint>( ( SquirrelVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<sbyte> SByteParam = new Param<sbyte>( ( SquirrelVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<short> ShortParam = new Param<short>( ( SquirrelVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<int> IntParam = new Param<int>( ( SquirrelVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<long> LongParam = new Param<long>( ( SquirrelVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<TileIndex> TileIndexParam = new Param<TileIndex>( ( SquirrelVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<bool> BoolParam = new Param<bool>( ( SquirrelVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<Enum> EnumParam = new Param<Enum>( ( SquirrelVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<ConvertibleThroughBase> ConvertibleThroughBaseParam = new Param<ConvertibleThroughBase>( ( SquirrelVM vm, int index ) => { SQ_GetInteger( vm, index, out SQInteger res ); return res; } );
    public static Param<string> StringParam = new Param<string>( ( SquirrelVM vm, int index ) =>
    {
        // Convert whatever there is as parameter to a string
        SQ_ToString( vm, index );

        SQString tmp;
        SQ_GetString( vm, -1, out tmp );
        string result = StrMakeValid( tmp );
        SQ_PopTo( vm );
        return result;
    } );

    // TODO: ARRAY PARAM

    public static SQInteger DefSQNonStaticCallback<TCls, TMethod, TType>( SquirrelVM vm )
        where TCls : class
        where TType : unmanaged, Enum
    {
        throw new NotImplementedException( "Can't be bothered rn..." );
    }


}