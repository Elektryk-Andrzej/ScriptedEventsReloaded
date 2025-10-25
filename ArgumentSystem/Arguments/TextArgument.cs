using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers;
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
        switch (token)
        {
            case TextToken { ContainsExpressions: false } textToken:
                return TryGet<string>.Success(textToken.Value);
            case TextToken textToken:
                return new(() => TryGet<string>.Success(textToken.ParsedValue()));
            case IValueCapableToken<LiteralValue> capableToken:
                return new(() => capableToken.ExactValue.OnSuccess(v => v.ToString()));
            default:
                Log.D($"{token.RawRep} | {token.GetType().Name}");
                return DynamicTryGet.Error($"Value '{token.RawRep}' cannot represent text.");
        }
    }
}