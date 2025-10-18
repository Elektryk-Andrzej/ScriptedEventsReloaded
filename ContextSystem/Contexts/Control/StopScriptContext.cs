using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Structures;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ContextSystem.Contexts.Control;

public class StopScriptContext: StandardContext, IKeywordContext
{
    public string KeywordName => "stop";

    public string Description =>
        "Stops the script from executing.";

    public string[] Arguments => [];
    
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Error(
            "'stop' keyword is not expecting any arguments after it.");
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    protected override void Execute()
    {
        Script.Stop();
    }
}