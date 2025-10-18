using JetBrains.Annotations;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Structures;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ContextSystem.Contexts.Control.Loops;

[UsedImplicitly]
public class LoopContinueContext : StandardContext, IKeywordContext
{
    public string KeywordName => "continue";
    public string Description =>
        "Makes a given loop (that the 'continue' keyword is inside) act as it has reached the end of its body.";
    public string[] Arguments => [];
    
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Error("The continue keyword does not expect arguments after it.");
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    protected override void Execute()
    {
        ParentContext?.SendControlMessage(ParentContextControlMessage.LoopContinue);
    }
}