using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Variables;

namespace SER.ArgumentSystem.Arguments;

public class LiteralVariableArgument(string name) : Argument(name)
{
    public override string InputDescription => "A literal variable name (doesnt have to be real)";
    
    [UsedImplicitly]
    public DynamicTryGet<LiteralVariableToken> GetConvertSolution(BaseToken token)
    {
        if (token is not LiteralVariableToken literalVariableToken)
        {
            return $"Value '{token.RawRep}' is not a syntactically valid literal variable.";
        }

        return literalVariableToken;
    }
}