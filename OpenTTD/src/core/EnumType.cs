using System;

namespace OpenTTD.Core;

/// <summary>
/// Enum-as-bit-set wrapper.<br/>
/// Allows wrapping enum values as a bit set. Methods are loosely modelled on bitset.<br/>
/// NOTE: Only set TEndValue if the bitset needs to be automatically masked to valid values.
/// </summary>
/// <typeparam name="TEnum">Enum values to wrap.</typeparam>
/// <typeparam name="TStorage">Storage type required to hold enum values.</typeparam>
/// <typeparam name="TEndValue">Last valid value + 1.</typeparam>
public class EnumBitSet<TEnum, TStorage, TEndValue> : BaseBitSet<EnumBitSet<TEnum, TStorage, TEndValue>, TEnum, TStorage> 
    where TEnum : Enum
{
    public EnumBitSet() : base()
    {

    }

    public EnumBitSet( TEnum value ) : base()
    {
        Set( value );
    }

    public EnumBitSet( TStorage data ) : base( data )
    {

    }

    public static int DecayValueType( dynamic value )
    {
        return 0;
    }
}