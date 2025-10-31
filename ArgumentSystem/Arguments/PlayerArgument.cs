using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.VariableTokens;
using SER.VariableSystem.Variables;

namespace SER.ArgumentSystem.Arguments;

public class PlayerArgument(string name) : Argument(name)
{
    public override string InputDescription => "Player variable e.g. @player, with EXACTLY 1 player.";

    [UsedImplicitly]
    public DynamicTryGet<Player> GetConvertSolution(BaseToken token)
    {
        if (token is not PlayerVariableToken playerVariableToken)
            return $"Value '{token.RawRep}' is not a player variable.";

        return new(() => DynamicSolver(playerVariableToken));
    }

    private TryGet<Player> DynamicSolver(PlayerVariableToken token)
    {
        if (Script.TryGetVariable<PlayerVariable>(token.Name).HasErrored(out var error, out var variable))
        {
            return error;
        }
        
        var plrs = variable.Players;
        if (plrs.Len != 1)
        {
            return $"The player variable '{token.RawRep}' must have exactly 1 player, but has {plrs.Len} instead.";
        }

        return plrs.First();
    }
}