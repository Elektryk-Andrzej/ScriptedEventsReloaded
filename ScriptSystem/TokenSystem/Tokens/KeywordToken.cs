using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Contexts.Control;
using SER.ScriptSystem.ContextSystem.Contexts.Loops;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.TokenSystem.Tokens;

public class KeywordToken : ContextableToken
{
    public override bool EndParsingOnChar(char c, out BaseToken? replaceToken)
    {
        replaceToken = null;
        return char.IsWhiteSpace(c);
    }

    public override Result IsValidSyntax()
    {
        return true;
    }

    public override TryGet<Context> TryGetResultingContext()
    {
        var info = (Script, LineNum);
        
        return RawRepresentation.ToLower() switch
        {
            "if" => Context.Create<IfStatementContext>(info),
            "end" => Context.Create<EndTreeContext>(info),
            "continue" => Context.Create<LoopContinueContext>(info),
            "repeat" => Context.Create<RepeatLoopContext>(info),
            "stop" => Context.Create<StopScriptContext>(info),
            "forever" => Context.Create<ForeverLoopContext>(info),
            "while" => Context.Create<WhileLoopContext>(info),
            "else" => Context.Create<ElseStatementContext>(info),
            "elif" => Context.Create<ElifStatementContext>(info),
            _ => $"Value '{RawRepresentation}' is not a keyword."
        };
    }
}