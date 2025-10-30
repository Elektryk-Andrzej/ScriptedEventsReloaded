using System;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Contexts;
using SER.Helpers.ResultSystem;
using SER.MethodSystem;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem;
using SER.TokenSystem.Structures;

namespace SER.TokenSystem.Tokens;

public class MethodToken : BaseToken, IContextableToken
{
    public Method Method { get; private set; } = null!;
    
    protected override IParseResult InternalParse(Script scr)
    {
        if (MethodIndex.TryGetMethod(Slice.RawRep).HasErrored(out _, out var method))
        {
            return new Ignore();
        }

        Method = (Method)Activator.CreateInstance(method.GetType());
        Method.Script = scr;
        return new Success();
    }

    public TryGet<Context> TryGetContext(Script scr)
    {
        return new MethodContext(this)
        {
            LineNum = LineNum,
            Script = scr,
        };
    }
}