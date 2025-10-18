using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ArgumentSystem.Arguments;

public class PlayerArgument(string name) : Argument(name)
{
    public override string InputDescription => "Player variable e.g. @player, with EXACTLY 1 player.";

    [UsedImplicitly]
    public DynamicTryGet<Player> GetConvertSolution(BaseToken token)
    {
        if (token is not PlayerVariableToken playerVariableToken)
            return $"Value '{token.RawRepresentation}' is not a player variable.";

        return new(() => DynamicSolver(playerVariableToken));
    }

    private TryGet<Player> DynamicSolver(PlayerVariableToken token)
    {
        if (Script.TryGetPlayerVariable(token.Name).HasErrored(out var error, out var variable))
        {
            return error;
        }
        
        var plrs = variable.Players;
        if (plrs.Count != 1)
        {
            return $"The player variable '{token.RawRepresentation}' must have exactly 1 player, but has {plrs.Count} instead.";
        }

        return plrs.First();
    }
}