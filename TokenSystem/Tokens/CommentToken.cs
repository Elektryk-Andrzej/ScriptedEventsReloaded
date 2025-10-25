using System.Linq;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Contexts;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Structures;

namespace SER.TokenSystem.Tokens;

public class CommentToken : BaseToken, IContextableToken
{
    protected override Result InternalParse(Script scr)
    {
        if (RawRep.FirstOrDefault() == '#') return true;
        
        return "Value is not a comment.";
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