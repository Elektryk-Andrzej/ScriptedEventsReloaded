using SER.ContextSystem.BaseContexts;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;

namespace SER.TokenSystem.Structures;

public interface IContextableToken
{
    public TryGet<Context> TryGetContext(Script scr);
}