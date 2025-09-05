using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.Control;

public class StopScriptContext: StandardContext, IKeywordContext
{
    public string Keyword => "stop";

    public string Description =>
        "Stops the script from executing.";

    public string? Arguments => null;
    
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