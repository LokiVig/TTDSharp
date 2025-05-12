namespace OpenTTD.Core;

public class BaseBitSet<TImpl, TValueType, TStorage> 
    where TImpl : BaseBitSet<TImpl, TValueType, TStorage>, new() 
    where TValueType : class 
    where TStorage : new()
{
    public TValueType ValueType;
    public TStorage BaseType;
    public static TStorage MASK = default;

    private TStorage data;

    public BaseBitSet()
    {
        data = default;
    }

    public BaseBitSet( TStorage data )
    {
        this.data = default /*new TStorage(data & MASK)*/;
    }

    public TImpl Set()
    {
        data = MASK;
        return (TImpl)this;
    }

    public TImpl Set( TValueType value )
    {
        data |= (1UL << TImpl.DecayValueType( value ));
        return (TImpl)this;
    }

    public TImpl Set( TValueType value, bool set )
    {
        return set ? Set( value ) : Reset( value );
    }

    public TImpl Reset( TValueType value )
    {
        data &= ~(1UL << TImpl.DecayValueType(value));
        return (TImpl)this;
    }

    public TImpl Flip( TValueType value )
    {
        if ( Test( value ) )
        {
            return Reset( value );
        }
        else
        {
            return Set( value );
        }
    }

    public bool Test( TValueType value )
    {
        return data & ( 1UL << TImpl.DecayValueType( value ) ) != 0;
    }

    public bool All( TImpl other )
    {
        return data & other.data == other.data;
    }

    public bool All()
    {
        return data == MASK;
    }

    public bool Any( TImpl other )
    {
        return data & other.data != 0;
    }

    public bool Any()
    {
        return data != 0;
    }

    public bool None()
    {
        return data == 0;
    }

    public static TImpl operator |( TImpl lhs, TImpl rhs )
    {
        TImpl ret = new TImpl();
        ret.data = lhs.data | rhs.data;

        return ret;
    }

    public static TImpl operator &( TImpl lhs, TImpl rhs )
    {
        TImpl ret = new TImpl();
        ret.data = lhs.data & rhs.data;

        return ret;
    }

    public TStorage Base()
    {
        return data;
    }

    public bool IsValid()
    {
        return Base() & MASK == Base();
    }
}