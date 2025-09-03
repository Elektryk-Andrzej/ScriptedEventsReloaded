using JetBrains.Annotations;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a percentage argument used in a method.
/// </summary>
public class PercentageArgument(string name) : GenericMethodArgument(name)
{
    public override string? AdditionalDescription => null;
        
    [UsedImplicitly]
    public ArgumentEvaluation<float> GetConvertSolution(BaseToken token)
    {
        return CustomConvertSolution(token, InternalConvert);
    }

    private ArgumentEvaluation<float>.EvalRes InternalConvert(string value)
    {
        if (!value.EndsWith("%"))
        {
            return Rs.Add($"Value '{value}' must end with '%' to be a percentage.");
        }
        
        value = value.Substring(0, value.Length - 1);

        if (!float.TryParse(value, out var result))
        {
            return Rs.Add($"Value '{value}' cannot be interpreted as a number.");
        }

        return result / 100;
    }
}