using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class BoolToken : LiteralValueToken<BoolValue>
{
    protected override Result InternalParse(Script scr)
    {
        if (bool.TryParse(Slice.RawRepresentation, out var res1))
        {
            Value = res1;
            return true;
        }
        
        return "Value is not a boolean.";
    }
}