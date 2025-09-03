using JetBrains.Annotations;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;
using SER.VariableSystem;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a text input argument used in a method.
/// </summary>
public class TextArgument(string name) : CustomMethodArgument(name)
{
    public override string? AdditionalDescription => null;
    public override string InputDescription => "Any text";

    [UsedImplicitly]
    public ArgumentEvaluation<string> GetConvertSolution(BaseToken token)
    {
        var value = token is TextToken text
            ? text.ValueWithoutBrackets
            : token.RawRepresentation;

        return VariableParser.IsValueSyntaxUsedInString(value, Script, out var getProcessedVariableValueFunc)
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