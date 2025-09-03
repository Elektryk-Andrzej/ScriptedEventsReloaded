using JetBrains.Annotations;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class PlayerVariableNameArgument(string name) : GenericMethodArgument(name)
{
    public override string AdditionalDescription =>
        "This argument expects a value that's a syntactically valid player variable BUT that variable name does not " +
        "(or sometimes shouldn't) have to be an actual variable.";
        
    [UsedImplicitly]
    public ArgumentEvaluation<PlayerVariableToken> GetConvertSolution(BaseToken token)
    {
        if (token is not PlayerVariableToken playerVariableToken)
        {
            return $"Value '{token.RawRepresentation}' is not a syntactically valid player variable.";
        }

        return playerVariableToken;
    }
}