using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a player collection argument used in a method.
/// </summary>
public class PlayersArgument(string name) : BaseMethodArgument(name)
{
    public override OperatingValue Input => OperatingValue.Players | OperatingValue.AllOfType;
    public override string? AdditionalDescription => null;
    
    public ArgumentEvaluation<List<Player>> GetConvertSolution(BaseToken token)
    {
        if (token.GetValue() is "*")
        {
            return new(() => Player.List.ToList());
        }
        
        if (token is not PlayerVariableToken playerVariableToken)
        {
            return new(
                $"Value '{token.RawRepresentation}' is not a player variable."
            );
        }

        return new(() => DynamicSolver(playerVariableToken));
    }

    private ArgumentEvaluation<List<Player>>.EvalRes DynamicSolver(PlayerVariableToken token)
    {
        if (Script.TryGetPlayerVariable(token.NameWithoutPrefix).HasErrored(out var error, out var variable))
        {
            return error;
        }

        return variable.Players();
    }
}