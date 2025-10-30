using SER.ScriptSystem;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class BoolToken : LiteralValueToken<BoolValue>
{
    protected override IParseResult InternalParse(Script scr)
    {
        if (bool.TryParse(Slice.RawRep, out var res1))
        {
            Value = res1;
            return new Success();
        }
        
        return new Ignore();
    }
}