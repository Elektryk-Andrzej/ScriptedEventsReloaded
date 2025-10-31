namespace SER.ValueSystem;

public class NumberValue(decimal value) : LiteralValue<decimal>(value)
{
    public static implicit operator NumberValue(decimal value)
    {
        return new(value);
    }

    public static implicit operator decimal(NumberValue value)
    {
        return value.ExactValue;
    }

    public override string StringRep => ExactValue.ToString();
}