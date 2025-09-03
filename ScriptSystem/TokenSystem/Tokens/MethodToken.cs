using System;
using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.MethodSystem;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Contexts;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.TokenSystem.Tokens;

public class MethodToken : ContextableToken
{
    public Method Method { get; set; } = null!;

    public override TryGet<Context> TryGetResultingContext()
    {
        if (!MethodIndex.NameToMethodIndex.TryGetValue(RawRepresentation, out var method))
            return $"There is no method named '{RawRepresentation}'.";

        if (Activator.CreateInstance(method.GetType()) is not Method createdMethod)
            return $"Method '{RawRepresentation}' couldn't be created.";

        Method = createdMethod;
        Method.Script = Script;
        
        return new MethodContext(this)
        {
            Script = Script,
            LineNum = LineNum
        };
    }

    public override bool EndParsingOnChar(char c, out BaseToken? replaceToken)
    {
        replaceToken = null;
        return char.IsWhiteSpace(c);
    }

    public override Result IsValidSyntax()
    {
        return true;
    }
}