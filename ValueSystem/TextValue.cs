namespace SER.ValueSystem;

public class TextValue(string value) : LiteralValue<string>(value)
{
    public static implicit operator TextValue(string value)
    {
        return new(value);
    }
    
    public static implicit operator string(TextValue value)
    {
        return value.ExactValue;
    }

    protected override string StringRep => ExactValue;
}