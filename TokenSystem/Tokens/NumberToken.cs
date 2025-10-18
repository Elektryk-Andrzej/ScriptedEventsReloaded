using SER.Helpers;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Structures;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class NumberToken : ValueToken<NumberValue>, ILiteralValueToken
{
    protected override Result InternalParse(Script scr)
    {
        if (decimal.TryParse(Slice.RawRepresentation, out var value))
        {
            Value = value;
            return true;
        }

        if (RawRepresentation.EndsWith("%") &&
            decimal.TryParse(RawRepresentation.Substring(0, RawRepresentation.Length - 1), out var value2))
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