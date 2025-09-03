using System;
using System.Linq;
using JetBrains.Annotations;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents an argument that consists of a set of predefined options.
/// The options specify the acceptable values for this argument.
/// </summary>
public class OptionsArgument(string name, params Option[] options) : CustomMethodArgument(name)
{
    public readonly Option[] Options = options;

    public override string InputDescription =>
        "One of the following values:\n * " +
        string.Join("\n * ", Options.Select(o => string.IsNullOrEmpty(o.Description) 
            ? o.Value
            : $"{o.Value} ({o.Description})"));

    [UsedImplicitly]
    public ArgumentEvaluation<string> GetConvertSolution(BaseToken token)
    {
        return CustomConvertSolution(token, InternalConvert);
    }

    private ArgumentEvaluation<string>.EvalRes InternalConvert(string value)
    {
        var option = Options.FirstOrDefault(opt => opt.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));
        if (option == null)
            return new()
            {
                Result = $"Value '{value}' does not match any of the following options: " +
                         $"{string.Join(", ", Options.Select(o => o.Value))}",
                Value = null!
            };

        return new()
        {
            Result = true,
            Value = option.Value
        };
    }
}