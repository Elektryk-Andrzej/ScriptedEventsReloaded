using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ArgumentSystem.Arguments;

public class PlayersArgument(string name) : Argument(name)
{
    public override string InputDescription => "Player variable e.g. @players or * for every player";

    [UsedImplicitly]
    public DynamicTryGet<List<Player>> GetConvertSolution(BaseToken token)
    {
        if (token is SymbolToken { IsJoker: true })
        {
            return new(() => Player.ReadyList.ToList());
        }
        
        if (token is not PlayerVariableToken playerVariableToken)
        {
            return $"Value '{token.RawRepresentation}' is not a player variable.";
        }

        return new(() => GetFromPlayerToken(playerVariableToken));
    }

    private TryGet<List<Player>> GetFromPlayerToken(PlayerVariableToken token)
    {
        if (Script.TryGetPlayerVariable(token.Name).HasErrored(out var error, out var variable))
        {
            return error;
        }

        return variable.Players;
    }
    
    /*private ArgumentEvaluation<List<Player>>.EvalRes GetFromLiteralToken(LiteralVariableToken token)
    {
        if (VariableParser.TryParseMethod(token.ValueWithoutBrackets, Script).HasErrored(out var error, out var method))
        {
            return error;
        }
        
        if (method is not PlayerReturningMethod methodWithReturn)
        {
            return $"Method '{token.GetValue()}' does not return a player.";
        }
        
        methodWithReturn.Execute();
        if (methodWithReturn.PlayerReturn is null)
        {
            throw new AndrzejFuckedUpException($"Method {methodWithReturn.Name} did not return a player value");
        }

        return methodWithReturn.PlayerReturn.ToList();
    }*/
}










