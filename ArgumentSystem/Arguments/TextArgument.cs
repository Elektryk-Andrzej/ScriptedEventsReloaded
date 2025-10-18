using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.VariableSystem.Variables;

namespace SER.ArgumentSystem.Arguments;

public class TextArgument(string name) : Argument(name)
{
    public override string InputDescription => "Any text e.g. \"Hello, World!\"";

    [UsedImplicitly]
    public DynamicTryGet<string> GetConvertSolution(BaseToken token)
    {
        Log.Debug("using text argument converter");
        if (token is LiteralVariableToken varToken)
        {
            return new(() => VariableHandling(varToken));
        }
        
        if (token is not TextToken textToken)
        {
            return DynamicTryGet.Success(token.GetBestTextRepresentation(Script));
        }
        
        if (!textToken.ContainsExpressions)
        {
            return DynamicTryGet.Success(textToken.Value.Value);
        }
            
        return new(() => TryGet<string>.Success(textToken.ParsedValue(Script)));
    }

    private static TryGet<string> VariableHandling(LiteralVariableToken token)
    {
        if (token.TryGetVariable().HasErrored(out var error, out var variable))
        {
            return TryGet<string>.Error(error);
        }

        if (variable is TextVariable textVariable)
        {
            return TryGet<string>.Success(textVariable.ExactValue.Value);
        }

        return TryGet<string>.Success(variable.BaseValue.Value.ToString());
    }
}