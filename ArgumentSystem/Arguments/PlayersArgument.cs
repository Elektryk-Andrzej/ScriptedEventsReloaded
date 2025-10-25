using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
using SER.TokenSystem.Tokens.Variables;
using SER.ValueSystem;

namespace SER.ArgumentSystem.Arguments;

public class PlayersArgument(string name) : Argument(name)
{
    public override string InputDescription => $"Player variable e.g. {PlayerVariableToken.Example} or * for every player";

    [UsedImplicitly]
    public DynamicTryGet<List<Player>> GetConvertSolution(BaseToken token)
    {
        if (token is SymbolToken { IsJoker: true })
        {
            return new(() => Player.ReadyList.ToList());
        }

        if (token is not IValueCapableToken<PlayerValue> valueCapableToken)
        {
            return $"Value '{token.RawRep}' does not represent a valid player variable.";
        }
        
        return new(() => valueCapableToken.ExactValue.OnSuccess(p => p.Players.ToList()));
    }
}










