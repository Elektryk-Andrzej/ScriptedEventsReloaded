using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Tokens;
using SER.VariableSystem.Variables;

namespace SER.ArgumentSystem.Arguments;

public class ReferenceArgument<TValue>(string name) : Argument(name)
{
    public override string InputDescription => $"A reference to {typeof(TValue).GetAccurateName()} object.";

    [UsedImplicitly]
    public DynamicTryGet<TValue> GetConvertSolution(BaseToken token)
    {
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (token is LiteralVariableToken varToken)
        {
            return new(() => LiteralVariableHandler(varToken));
        }

        if (token is LiteralExpressionToken exprToken)
        {
            return new(() => LiteralExpressionHandler(exprToken, Script));
        }
        
        return $"Value '{token.RawRepresentation}' does not represent a valid reference.";
    }

    public static TryGet<TValue> TryParse(BaseToken token, Script script)
    {
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (token is LiteralVariableToken varToken)
        {
            return LiteralVariableHandler(varToken);
        }

        if (token is LiteralExpressionToken exprToken)
        {
            return LiteralExpressionHandler(exprToken, script);
        }
        
        return $"Value '{token.RawRepresentation}' does not represent a valid reference.";
    }

    private static TryGet<TValue> LiteralExpressionHandler(LiteralExpressionToken token, Script script)
    {
        if (token.GetLiteralValue(script).HasErrored(out var error, out var value))
        {
            return error;
        }

        if (value is not TValue correctValue)
        {
            return $"Expression resulted in a value of type {value.GetType().GetAccurateName()} " +
                   $"which is not compatible with expected reference to {typeof(TValue).GetAccurateName()}.";
        }
        
        return correctValue;
    }

    private static TryGet<TValue> LiteralVariableHandler(LiteralVariableToken token)
    {
        if (token.Variable is null)
        {
            return $"There is no {token.RawRepresentation} variable defined";
        }
        
        if (token.Variable is not ReferenceVariable refVar)
        {
            return $"Variable {token.RawRepresentation} does not hold a reference.";
        }

        if (refVar.ExactValue.Value is not TValue correctValue)
        {
            return $"The reference held by variable {token.RawRepresentation} " +
                   $"({refVar.ExactValue.Value.GetType().GetAccurateName()}) is not compatible with expected reference " +
                   $"to {typeof(TValue).GetAccurateName()}.";
        }
        
        return correctValue;
    }
}