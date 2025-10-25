using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Contexts;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Structures;

namespace SER.TokenSystem.Tokens;

public class FlagArgumentToken : BaseToken, IContextableToken
{
    protected override Result InternalParse(Script scr)
    {
        if (Slice.RawRepresentation == "--") return true;
        
        return $"Value '{RawRep}' is not a valid flag argument beginning '--'";
    }

    public TryGet<Context> TryGetContext(Script scr)
    {
        return new NoOperationContext
        {
            Script = Script,
            LineNum = LineNum,
        };
    }
}