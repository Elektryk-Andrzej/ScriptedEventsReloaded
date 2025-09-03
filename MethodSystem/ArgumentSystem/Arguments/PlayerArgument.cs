using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a single player object argument used in a method.
/// </summary>
public class PlayerArgument(string name) : CustomMethodArgument(name)
{
    public override string InputDescription => "player variable with execatly 1 player.";
    public override string? AdditionalDescription => null;

    [UsedImplicitly]
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
        
        var plrs = variable.Players;
        if (plrs.Count != 1)
        {
            return Rs.Add($"The player variable '{token.RawRepresentation}' must have exactly 1 player, but has {plrs.Count} instead.");
        }

        return plrs.First();
    }
}