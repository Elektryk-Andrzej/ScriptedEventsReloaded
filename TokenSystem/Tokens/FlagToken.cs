using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Contexts;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Structures;

namespace SER.TokenSystem.Tokens;

public class FlagToken : BaseToken, IContextableToken
{
    protected override IParseResult InternalParse(Script scr)
    {
        return Slice.RawRep == "!--"
            ? new Success()
            : new Ignore();
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