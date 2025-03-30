using System;
using System.Numerics;

namespace OpenTTD.Core;

public static class BitMath
{
    /// <summary>
    /// Fetch <paramref name="n"/> bits from <paramref name="x"/>, started at bit <paramref name="s"/>.<br/>
    /// <br/>
    /// This function can be used to fetch <paramref name="n"/> bits from the value <paramref name="x"/>. The<br/>
    /// <paramref name="s"/> value sets the start position to read. The start position is<br/>
    /// count from the LSB and starts at <c>0</c>. The result starts at a<br/>
    /// LSB, as this isn't just an and-bitmask but also some bit-shifting operations.<br/>
    /// <c>GB(0xFF, 2, 1)</c> will so return <c>0x01 (0000 0001)</c> instead of <c>0x04 (0000 01000).</c>
    /// </summary>
    /// <param name="x">The value to read some bits.</param>
    /// <param name="s">The start position to read some bits.</param>
    /// <param name="n">The number of bits to read.</param>
    /// <returns>The selected bits, aligned to a LSB.</returns>
    public static uint GB( dynamic x, byte s, byte n )
    {
        return ( x >> s ) & ( ( (dynamic)1U << n ) - 1 );
    }

    /// <summary>
    /// Set <paramref name="n"/> bits in <paramref name="x"/> starting at bit <paramref name="s"/> to <paramref name="d"/>.<br/>
    /// <br/>
    /// This function sets <paramref name="n"/> bits from <paramref name="x"/> which started at bit <paramref name="s"/> to the value of <paramref name="d"/>.<br/>
    /// The parameters <paramref name="x"/>, <paramref name="s"/> and <paramref name="n"/> works the same as the parameters of <see cref="GB(dynamic, byte, byte)"/>.<br/>
    /// The result is saved in <paramref name="x"/> again. Unused bits in the window provided by <paramref name="n"/> are set to 0 if the value of<br/>
    /// <paramref name="d"/> isn't "big" enough.<br/>
    /// This is not a bug, it's a feature.<br/>
    /// <br/>
    /// <para>
    ///     <b>NOTE:</b> Parameter <paramref name="x"/> must be a variable as the result is saved there.<br/>
    ///     <b>NOTE:</b> To avoid unexpected results the value of <paramref name="d"/> should not use more space<br/>
    ///           as the provided space of <paramref name="n"/> bits (log2).
    /// </para>
    /// </summary>
    /// <param name="x">The variable to change some bits.</param>
    /// <param name="s">The start position for the new bits.</param>
    /// <param name="n">The size / window for the new bits.</param>
    /// <param name="d">The actually new bits to save in the defined position.</param>
    /// <returns>The new value of <paramref name="x"/>.</returns>
    public static dynamic SB( dynamic x, byte s, byte n, dynamic d )
    {
        x &= ~( ( ( (dynamic)1U << n ) - 1 ) << s );
        x |= d << s;
        return x;
    }

    /// <summary>
    /// Add <paramref name="i"/> to <paramref name="n"/> bits of <paramref name="x"/> starting at bit <paramref name="s"/>.<br/>
    /// <br/>
    /// This adds the value of <paramref name="i"/> on <paramref name="n"/> bits of <paramref name="x"/> starting at bit <paramref name="s"/>.<br/>
    /// The parameters <paramref name="x"/>, <paramref name="s"/>, <paramref name="i"/> are similar to <see cref="GB(dynamic, byte, byte)"/>. Besides, <paramref name="x"/> must be a variable<br/>
    /// as the result are saved there. An overflow does not affect the following bits of the given bit window and is simply ignored.<br/>
    /// <br/>
    /// <para>
    ///     <b>NOTE:</b> Parameter <paramref name="x"/> must be a variable as the result is saved there.
    /// </para>
    /// </summary>
    /// <param name="x">The variable to add some bits at some position.</param>
    /// <param name="s">The start position of the addition.</param>
    /// <param name="n">The size / window for the addition.</param>
    /// <param name="i">The value to add at the given start position in the given window.</param>
    /// <returns>The new value of <paramref name="x"/>.</returns>
    public static dynamic AB( dynamic x, byte s, byte n, dynamic i )
    {
        dynamic mask = ( ( (dynamic)1U << n ) - 1 ) << s;
        x = ( x & ~mask ) | ( ( x + ( i << s ) ) & mask );
        return x;
    }

    /// <summary>
    /// Checks if a bit in a value is set.<br/>
    /// <br/>
    /// This function checks if a bit inside a value is set or not.<br/>
    /// The <paramref name="y"/> value specific the position of the bit, started at the<br/>
    /// LSB and count from 0.
    /// </summary>
    /// <param name="x">The value to check.</param>
    /// <param name="y">The position of the bit to check, started from the LSB.</param>
    /// <returns><see langword="true"/> if the bit is set, <see langword="false"/> else.</returns>
    public static bool HasBit( dynamic x, byte y )
    {
        return ( x & ( (dynamic)1U << y ) ) != 0;
    }

    /// <summary>
    /// Set a bit in a variable.<br/>
    /// <br/>
    /// This function sets a bit in a variable. The variable is changed<br/>
    /// and the value is also returned. Parameter <paramref name="y"/> defines the bit and<br/>
    /// starts at the LSB with 0.
    /// </summary>
    /// <param name="x">The variable to set a bit.</param>
    /// <param name="y">The bit position to set.</param>
    /// <returns>The new value of the old value with the bit set.</returns>
    public static dynamic SetBit( dynamic x, byte y )
    {
        return x = x | ( (dynamic)1U << y );
    }

    /// <summary>
    /// Sets several bits in a variable.<br/>
    /// <br/>
    /// This macro sets several bits in a variable. The bits to set are provided<br/>
    /// by a value. The new value is also returned.
    /// </summary>
    /// <param name="x">The variable to set some bits.</param>
    /// <param name="y">The value with set bits for setting them in the variable.</param>
    /// <returns>The new value of <paramref name="x"/>.</returns>
    public static dynamic SETBITS( dynamic x, byte y ) { return x |= y; }

    /// <summary>
    /// Clears a bit in a variable.<br/>
    /// <br/>
    /// This function clears a bit in a variable. The variable is<br/>
    /// changed and the value is also returned. Parameter <paramref name="y"/> defines the bit<br/>
    /// to clear and starts at the LSB with 0.
    /// </summary>
    /// <param name="x">The variable to clear the bit.</param>
    /// <param name="y">The bit position to clear.</param>
    /// <returns>The new value of the old value with the bit cleared.</returns>
    public static dynamic ClrBit( dynamic x, byte y )
    {
        return x = x & ~( (dynamic)1U << y );
    }

    /// <summary>
    /// Clears several bits in a variable.<br/>
    /// <br/>
    /// This macro clears several bits in a variable. The bits to clear are<br/>
    /// provided by a value. The new value is also returned.
    /// </summary>
    /// <param name="x">The variable to clear some bits.</param>
    /// <param name="y">The value with set bits for clearing them in the variable.</param>
    /// <returns>The new value of <paramref name="x"/>.</returns>
    public static dynamic CLRBITS( dynamic x, byte y ) { return x &= ~y; }

    /// <summary>
    /// Toggles a bit in a variable.<br/>
    /// <br/>
    /// This function toggles a bit in a variable. The variable is<br/>
    /// changed and the value is also returned. Parameter <paramref name="y"/> defines the bit<br/>
    /// to toggle and starts at the LSB with 0.
    /// </summary>
    /// <param name="x">The variable to toggle the bit.</param>
    /// <param name="y">The bit position to toggle.</param>
    /// <returns>The new value of the old value with the bit toggled.</returns>
    public static dynamic ToggleBit( dynamic x, byte y )
    {
        return x = x ^ ( (dynamic)1U << y );
    }

    /// <summary>
    /// Assigns a bit in a variable.<br/>
    /// <br/>
    /// This function assigns a single bit in a variable. The variable is<br/>
    /// changed and the value is also returned. Parameter <paramref name="y"/> defines the bit<br/>
    /// to assign and starts at the LSB with 0.
    /// </summary>
    /// <param name="x">The variable to assign the bit.</param>
    /// <param name="y">The bit position to assign.</param>
    /// <param name="value">The new bit value.</param>
    /// <returns>The new value of the old value with the bit assigned.</returns>
    public static dynamic AssignBit( dynamic x, byte y, bool value )
    {
        return SB( x, y, 1, value ? 1 : 0 );
    }

    /// <summary>
    /// Search the first set bit in a value.<br/>
    /// When no bit is set, it returns 0.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="x">The value to search.</param>
    /// <returns>The position of the first bit set.</returns>
    public static byte FindFirstBit<T>( T x )
        where T : unmanaged
    {
        if ( x is Enum )
        {
            return (byte)BitOperations.TrailingZeroCount( Convert.ToUInt64( x ) );
        }

        return (byte)BitOperations.TrailingZeroCount( (dynamic)x );
    }

    /// <summary>
    /// Search the last bit in a value.<br/>
    /// When no bit is set, it returns 0.
    /// </summary>
    /// <param name="x">The value to search.</param>
    /// <returns>The position of the last bit set.</returns>
    public static byte FindLastBit( dynamic x )
    {
        if ( x == 0 )
        {
            return 0;
        }

        return x == 0 ? -1 : BitOperations.Log2( x );
    }

    /// <summary>
    /// Clear the first bit in an integer.<br/>
    /// <br/>
    /// This function returns a value where the first bit (from LSB)<br/>
    /// is cleared.<br/>
    /// So, <c>110100</c> returns <c>110000</c>, <c>0000001</c> returns <c>0000000</c>, etc.
    /// </summary>
    /// <param name="value">The value to clear the first bit.</param>
    /// <returns>The new value with the first bit cleared.</returns>
    public static byte KillFirstBit( dynamic value )
    {
        return value &= value - 1;
    }

    /// <summary>
    /// Counts the number of set bits in a variable.
    /// </summary>
    /// <param name="value">The value to count the number of bits in.</param>
    /// <returns>The number of bits.</returns>
    public static uint CountBits( dynamic value )
    {
        return BitOperations.PopCount( value );
    }

    /// <summary>
    /// Test whether <paramref name="value"/> has exactly 1 bit set.
    /// </summary>
    /// <param name="value">The value to test.</param>
    /// <returns>Does <paramref name="value"/> have exactly 1 bit set?</returns>
    public static bool HasExactlyOneBit( dynamic value )
    {
        return value != 0 && ( value & ( value - 1 ) ) == 0;
    }

    /// <summary>
    /// Test whether <paramref name="value"/> has at most 1 bit set.
    /// </summary>
    /// <param name="value">The value to test.</param>
    /// <returns>Does <paramref name="value"/> have at most 1 bit set?</returns>
    public static bool HasAtMostOneBit( dynamic value )
    {
        return value & ( value - 1 ) == 0;
    }

    /// <summary>
    /// Custom implementation of ByteSwap.<br/>
    /// Perform an endianness bitmap on <paramref name="x"/>.
    /// </summary>
    /// <param name="x">The variable to bitswap.</param>
    /// <returns>The bitswapped value.</returns>
    public static T ByteSwap<T>( T x )
        where T : unmanaged
    {
        unsafe
        {
            if ( sizeof( T ) == 1 )
            {
                return x;
            }

            if ( sizeof( T ) == 2 )
            {
                return ( (dynamic)x >> 8 ) | ( (dynamic)x << 8 );
            }

            if ( sizeof( T ) == 4 )
            {
                return ( ( (dynamic)x >> 24 ) & 0xFF ) | ( ( (dynamic)x >> 8 ) & 0xFF00 ) | ( ( (dynamic)x << 8 ) & 0xFF0000 ) | ( ( (dynamic)x << 24 ) & 0xFF000000 );
            }
        }

        return default;
    }
}

/// <summary>
/// Iterate ensemble of each set bit in a value.
/// </summary>
public struct SetBitIterator
{
    private uint bitset;

    public SetBitIterator( uint bitset )
    {
        this.bitset = bitset;
    }

    public struct Iterator
    {
        public uint valueType;
        public dynamic differenceType;

        private uint bitset;
        private uint bitpos;

        public Iterator( uint bitset )
        {
            this.bitset = bitset;
            bitpos = 0;
            Validate();
        }

        public static bool operator ==( Iterator lhs, Iterator rhs )
        {
            return lhs.bitset == rhs.bitset;
        }

        public static bool operator !=( Iterator lhs, Iterator rhs )
        {
            return lhs.bitset != rhs.bitset;
        }

        public static Iterator operator +( Iterator iterator )
        {
            iterator.Next();
            iterator.Validate();
            return iterator;
        }

        private void Validate()
        {
            if ( bitset != 0 )
            {
                uint unsignedValue = bitset;
                bitpos = BitMath.FindFirstBit( unsignedValue );
            }
        }

        private void Next()
        {
            bitset = BitMath.KillFirstBit( bitset );
        }
    }

    public Iterator Begin()
    {
        return new Iterator( bitset );
    }

    public Iterator End()
    {
        return new Iterator( 0 );
    }

    public bool Empty()
    {
        return Begin() == End();
    }
}