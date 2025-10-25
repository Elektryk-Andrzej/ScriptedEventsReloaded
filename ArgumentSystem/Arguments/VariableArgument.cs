using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Variables;
using SER.VariableSystem.Bases;

namespace SER.ArgumentSystem.Arguments;

/// <summary>
/// Represents any Variable argument used in a method.
/// </summary>
public class VariableArgument(string name) : Argument(name)
{
    public override string InputDescription => "Any existing variable e.g. $name or @players";

    [UsedImplicitly]
    public DynamicTryGet<Variable> GetConvertSolution(BaseToken token)
    {
        if (token is not VariableToken variableToken)
        {
            return $"Value '{token.RawRep}' is not a variable.";
        }

        return new(() => variableToken.TryGetVariable());
    }
}