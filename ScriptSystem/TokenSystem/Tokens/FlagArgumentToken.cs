using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Contexts;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.TokenSystem.Tokens;

public class FlagArgumentToken : BaseContextableToken
{
    public override bool EndParsingOnChar(char c)
    {
        return char.IsWhiteSpace(c);
    }

    public override Result IsValidSyntax()
    {
        return Result.Assert(RawRepresentation == "--",
            $"Flag argument prefix must be '--', not '{RawRepresentation}'");
    }

    public override TryGet<BaseContext> TryGetResultingContext()
    {
        return new NoOperationContext()
        {
            Script = Script,
            LineNum = LineNum
        };
    }
}