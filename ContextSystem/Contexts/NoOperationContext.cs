using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Structures;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ContextSystem.Contexts;

public class NoOperationContext : StandardContext
{
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Continue();
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    protected override void Execute()
    {
    }
}