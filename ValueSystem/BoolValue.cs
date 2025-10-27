namespace SER.ValueSystem;

public class BoolValue(bool value) : LiteralValue<bool>(value)
{
    public static implicit operator BoolValue(bool value)
    {
        return new(value);
    }
    
    public static implicit operator bool(BoolValue value)
    {
        return value.ExactValue;
    }

    protected override string StringRep => ExactValue.ToString().ToLower();
}