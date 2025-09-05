using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.Control.Loops;

public class LoopContinueContext : StandardContext, IKeywordContext
{
    public string Keyword => "continue";

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