using System;
using System.Linq;
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
    
    protected override Result InternalParse(Script scr)
    {
        Result error = $"Method failed while parsing from '{RawRep}'";

        if (!char.IsUpper(Slice.RawRepresentation.First()))
        {
            return error + "First character must be uppercase.";
        }

        if (MethodIndex.TryGetMethod(Slice.RawRepresentation).HasErrored(out var err, out var method))
        {
            return error + err;
        }

        Method = (Method)Activator.CreateInstance(method.GetType());
        Method.Script = scr;
        return true;
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