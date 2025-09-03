using JetBrains.Annotations;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens.LiteralVariables;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class LiteralVariableNameArgument(string name) : GenericMethodArgument(name)
{
    public override string? AdditionalDescription => 
        "This argument expects a value that's a syntactically valid literal variable BUT that variable name does not " +
        "(or sometimes shouldn't) have to be an actual variable.";
        
    [UsedImplicitly]
    public ArgumentEvaluation<LiteralVariableToken> GetConvertSolution(BaseToken token)
    {
        if (token is not LiteralVariableToken literalVariableToken)
        {
            return $"Value '{token.RawRepresentation}' is not a syntactically valid literal variable.";
        }

        return literalVariableToken;
    }
}