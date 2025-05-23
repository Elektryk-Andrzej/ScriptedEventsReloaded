using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Contexts.Control;
using SER.ScriptSystem.ContextSystem.Contexts.Loops;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.TokenSystem.Tokens;

public class KeywordToken : BaseContextableToken
{
    public override bool EndParsingOnChar(char c)
    {
        return char.IsWhiteSpace(c);
    }

    public override Result IsValidSyntax()
    {
        return true;
    }

    public override TryGet<BaseContext> TryGetResultingContext()
    {
        var info = (Script, LineNum);
        
        return RawRepresentation.ToLower() switch
        {
            "if" => BaseContext.Create<IfStatementContext>(info),
            "for" => BaseContext.Create<ForLoopContext>(info),
            "end" => BaseContext.Create<TerminationContext>(info),
            "continue" => BaseContext.Create<LoopContinueContext>(info),
            "repeat" => BaseContext.Create<RepeatLoopContext>(info),
            "stop" => BaseContext.Create<StopScriptContext>(info),
            "forever" => BaseContext.Create<ForeverLoopContext>(info),
            "while" => BaseContext.Create<WhileLoopContext>(info),
            _ => $"Value '{RawRepresentation}' is not a keyword."
        };
    }
}