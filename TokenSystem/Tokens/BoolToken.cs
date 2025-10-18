using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Structures;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class BoolToken : ValueToken<BoolValue>, ILiteralValueToken
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

    public TryGet<string> TextRepresentation(Script scr)
    {
        return TryGet<string>.Success(Value.ToString());
    }
}