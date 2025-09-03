using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Contexts;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.TokenSystem.Tokens;

public class FlagToken : ContextableToken
{
    public override bool EndParsingOnChar(char c, out BaseToken? replaceToken)
    {
        replaceToken = null;
        return char.IsWhiteSpace(c);
    }

    public override Result IsValidSyntax()
    {
        return Result.Assert(
            RawRepresentation == "!--",
            $"Script flag should start with '!--', not '{RawRepresentation}'.");
    }

    public override TryGet<Context> TryGetResultingContext()
    {
        return new NoOperationContext
        {
            Script = Script,
            LineNum = LineNum
        };
    }
}