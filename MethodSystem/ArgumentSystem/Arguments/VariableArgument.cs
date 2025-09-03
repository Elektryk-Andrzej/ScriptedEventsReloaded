﻿using JetBrains.Annotations;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;
using SER.ScriptSystem.TokenSystem.Tokens.LiteralVariables;
using SER.VariableSystem.Structures;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents any IVariable argument used in a method.
/// </summary>
public class VariableArgument(string name) : CustomMethodArgument(name)
{
    public override string? AdditionalDescription => null;
    public override string InputDescription => "Any variable";

    [UsedImplicitly]
    public ArgumentEvaluation<IVariable> GetConvertSolution(BaseToken token)
    {
        return token switch
        {
            PlayerVariableToken playerVariableToken => 
                new(() => DynamicPlayerVarSolver(playerVariableToken)),
            LiteralVariableToken literalVariableToken => 
                new(() => DynamicLiteralVarSolver(literalVariableToken)),
            _ => new($"Value '{token.RawRepresentation}' is not a valid variable.")
        };
    }

    private ArgumentEvaluation<IVariable>.EvalRes DynamicPlayerVarSolver(PlayerVariableToken token)
    {
        if (Script.TryGetPlayerVariable(token.NameWithoutPrefix).HasErrored(out var error, out var variable))
        {
            return Rs.Add(error);
        }

        return variable;
    }

    private ArgumentEvaluation<IVariable>.EvalRes DynamicLiteralVarSolver(LiteralVariableToken token)
    {
        if (Script.TryGetLiteralVariable(token.ValueWithoutBrackets).HasErrored(out var error, out var variable))
        {
            return Rs.Add(error);
        }

        return variable;
    }
}