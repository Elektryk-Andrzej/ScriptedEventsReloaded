using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Structures;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ContextSystem.Contexts.Control;

public class EndStatementContext : StandardContext, IKeywordContext
{
    public string KeywordName => "end";
    public string Description => "Ends the current statement's body.";
    public string[] Arguments => [];
    
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Error("There can't be anything else on the same line as the 'end' keyword.");
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    protected override void Execute()
    {
    }
}