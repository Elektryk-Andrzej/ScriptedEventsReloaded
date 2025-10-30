using SER.ScriptSystem;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class NumberToken : LiteralValueToken<NumberValue>
{
    protected override IParseResult InternalParse(Script scr)
    {
        if (decimal.TryParse(RawRep, out var value))
        {
            Value = value;
            return new Success();
        }

        if (RawRep.EndsWith("%") &&
            decimal.TryParse(RawRep.Substring(0, RawRep.Length - 1), out var value2))
        {
            Value = value2 / 100;
            return new Success();
        }

        return new Ignore();
    }
}