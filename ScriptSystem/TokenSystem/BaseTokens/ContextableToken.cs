using System.Diagnostics.Contracts;
using SER.Helpers;
using SER.ScriptSystem.ContextSystem.BaseContexts;

namespace SER.ScriptSystem.TokenSystem.BaseTokens;

public abstract class ContextableToken : BaseToken
{
    [Pure]
    public abstract TryGet<Context> TryGetResultingContext();
}