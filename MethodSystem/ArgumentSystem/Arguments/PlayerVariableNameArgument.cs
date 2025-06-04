using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class PlayerVariableNameArgument(string name) : BaseMethodArgument(name)
{
    public override OperatingValue Input => OperatingValue.PlayerVariableName;
    public override string AdditionalDescription =>
        "This argument expects a value that's a syntactically valid player variable BUT that variable name does not " +
        "have to be an actual variable.";
    
    public ArgumentEvaluation<PlayerVariableToken> GetConvertSolution(BaseToken token)
    {
        if (token is not PlayerVariableToken playerVariableToken)
        {
            return $"Value '{token.RawRepresentation}' is not a syntactically valid player variable.";
        }

        return playerVariableToken;
    }
}