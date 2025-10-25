using SER.Helpers;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class NumberToken : LiteralValueToken<NumberValue>
{
    protected override Result InternalParse(Script scr)
    {
        if (decimal.TryParse(Slice.RawRepresentation, out var value))
        {
            Value = value;
            return true;
        }

        if (RawRep.EndsWith("%") &&
            decimal.TryParse(RawRep.Substring(0, RawRep.Length - 1), out var value2))
        {
            Value = value2 / 100;
            return true;
        }

        return "Value is not a number.";
    }

    public TryGet<string> TextRepresentation(Script _)
    {
        Log.Debug($"value of number token is {Value} (string as {Value}");
        return TryGet<string>.Success(Value.ToString());
    }
}