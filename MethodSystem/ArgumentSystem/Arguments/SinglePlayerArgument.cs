using System.Linq;
using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a single player object argument used in a method.
/// </summary>
public class SinglePlayerArgument(string name) : BaseMethodArgument(name)
{
    public override OperatingValue Input => OperatingValue.Player;
    public override string? AdditionalDescription => null;
    
    public ArgumentEvaluation<Player> GetConvertSolution(BaseToken token)
    {
        if (token is not PlayerVariableToken playerVariableToken)
            return $"Value '{token.RawRepresentation}' is not a player variable.";

        return new(() => DynamicSolver(playerVariableToken));
    }

    private ArgumentEvaluation<Player>.EvalRes DynamicSolver(PlayerVariableToken token)
    {
        if (Script.TryGetPlayerVariable(token.NameWithoutPrefix).HasErrored(out var error, out var variable))
        {
            return error;
        }
        
        var plrs = variable.Players();
        if (plrs.Count != 1)
        {
            return $"The player variable must have exactly 1 player, but has {plrs.Count} instead.";
        }

        return plrs.First();
    }
}