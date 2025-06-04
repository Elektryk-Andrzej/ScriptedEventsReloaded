using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;
using SER.VariableSystem;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a text input argument used in a method.
/// </summary>
public class TextArgument(string name) : BaseMethodArgument(name)
{
    public override OperatingValue Input => OperatingValue.Text;

    public override string? AdditionalDescription => null;

    public ArgumentEvaluation<string> GetConvertSolution(BaseToken token)
    {
        var value = token is ParenthesesToken parentheses
            ? parentheses.ValueWithoutBrackets
            : token.RawRepresentation;

        return VariableParser.IsVariableUsedInString(value, Script, out var getProcessedVariableValueFunc)
            ? new(() => new()
            {
                Result = true,
                Value = getProcessedVariableValueFunc()
            })
            : new(new ArgumentEvaluation<string>.EvalRes
            {
                Result = true,
                Value = value
            });
    }
}