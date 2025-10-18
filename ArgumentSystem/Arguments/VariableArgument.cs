using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.VariableSystem.Variables;

namespace SER.ArgumentSystem.Arguments;

/// <summary>
/// Represents any IVariable argument used in a method.
/// </summary>
public class VariableArgument(string name) : Argument(name)
{
    public override string InputDescription => "Any existing variable e.g. $name or @players";

    [UsedImplicitly]
    public DynamicTryGet<IVariable> GetConvertSolution(BaseToken token)
    {
        return token switch
        {
            PlayerVariableToken playerVar => 
                new(() => DynamicPlayerVarSolver(playerVar)),
            
            LiteralExpressionToken { Type: VariableExpression varExpr } => 
                new(() => DynamicLiteralVarSolver(varExpr.Token)),
            
            LiteralVariableToken literalVar => 
                new(() => DynamicLiteralVarSolver(literalVar)),
            
            _ => new($"Value '{token.RawRepresentation}' is not a valid variable.")
        };
    }

    private TryGet<IVariable> DynamicPlayerVarSolver(PlayerVariableToken token)
    {
        if (Script.TryGetPlayerVariable(token.Name).HasErrored(out var error, out var variable))
        {
            return error;
        }

        return variable;
    }

    private TryGet<IVariable> DynamicLiteralVarSolver(LiteralVariableToken token)
    {
        if (Script.TryGetLiteralVariable(token)
            .HasErrored(out var error, out var variable))
        {
            return error;
        }

        return variable;
    }
}