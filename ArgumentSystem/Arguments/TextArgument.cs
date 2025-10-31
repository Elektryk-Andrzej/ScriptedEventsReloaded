using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;

namespace SER.ArgumentSystem.Arguments;

public class TextArgument(string name) : Argument(name)
{
    public override string InputDescription => "Any text e.g. \"Hello, World!\"";

    [UsedImplicitly]
    public DynamicTryGet<string> GetConvertSolution(BaseToken token)
    {
        if (token is TextToken textToken)
        {
            return textToken.GetDynamicResolver();
        }    
        
        if (token is not IValueToken valToken || !valToken.CanReturn<LiteralValue>(out var get))
        {
            return DynamicTryGet.Error($"Value '{token.RawRep}' cannot represent text.");
        }

        if (valToken.IsConstant)
        {
            return get().OnSuccess(v => v.StringRep);
        }

        return new(() => get().OnSuccess(v => v.StringRep));
    }
}