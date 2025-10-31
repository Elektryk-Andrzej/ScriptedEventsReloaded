using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.VariableTokens;

namespace SER.ArgumentSystem.Arguments;

public class PlayerVariableNameArgument(string name) : Argument(name)
{
    public override string InputDescription => "A player variable name (doesnt have to be real)";

    [UsedImplicitly]
    public DynamicTryGet<PlayerVariableToken> GetConvertSolution(BaseToken token)
    {
        if (token is not PlayerVariableToken playerVariableToken)
        {
            return $"Value '{token.RawRep}' is not a syntactically valid player variable.";
        }

        return playerVariableToken;
    }
}