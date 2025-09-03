using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using SER.Helpers.Exceptions;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;
using SER.ScriptSystem.TokenSystem.Tokens.LiteralVariables;
using SER.VariableSystem;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a player collection argument used in a method.
/// </summary>
public class PlayersArgument(string name) : CustomMethodArgument(name)
{
    public override string? AdditionalDescription => null;
    public override string InputDescription => "player variable";

    [UsedImplicitly]
    public ArgumentEvaluation<List<Player>> GetConvertSolution(BaseToken token)
    {
        if (token.GetValue() is "*")
        {
            return new(() => Player.ReadyList.ToList());
        }

        if (token is LiteralVariableToken literalVariable)
        {
            return new(() => GetFromLiteralToken(literalVariable));
        }
        
        if (token is not PlayerVariableToken playerVariableToken)
        {
            return new(
                $"Value '{token.RawRepresentation}' is not a player variable."
            );
        }

        return new(() => GetFromPlayerToken(playerVariableToken));
    }

    private ArgumentEvaluation<List<Player>>.EvalRes GetFromPlayerToken(PlayerVariableToken token)
    {
        if (Script.TryGetPlayerVariable(token.NameWithoutPrefix).HasErrored(out var error, out var variable))
        {
            return Rs.Add(error);
        }

        return variable.Players;
    }
    
    private ArgumentEvaluation<List<Player>>.EvalRes GetFromLiteralToken(LiteralVariableToken token)
    {
        if (VariableParser.TryParseMethod(token.ValueWithoutBrackets, Script).HasErrored(out var error, out var method))
        {
            return Rs.Add(error);
        }
        
        if (method is not PlayerReturningMethod methodWithReturn)
        {
            return Rs.Add($"Method '{token.GetValue()}' does not return a player.");
        }
        
        methodWithReturn.Execute();
        if (methodWithReturn.PlayerReturn is null)
        {
            throw new AndrzejFuckedUpException($"Method {methodWithReturn.Name} did not return a player value");
        }

        return methodWithReturn.PlayerReturn.ToList();
    }
}










