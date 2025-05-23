using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a float amount argument used in a method.
/// <remarks>
/// This argument is specifically for *amount*. If you want to just get a number, use <see cref="NumberArgument"/>.
/// </remarks>
/// </summary>
public class FloatAmountArgument(string name, float minValue) : BaseMethodArgument(name)
{
    public override OperatingValue Input => OperatingValue.Float;

    public override string AdditionalDescription =>
        $"Value must be higher than {minValue} e.g. {minValue + 1.3f}";


    public ArgumentEvaluation<float> GetConvertSolution(BaseToken token)
    {
        return DefaultConvertSolution<float>(token, new()
        {
            [OperatingValue.Float] = o =>
            {
                var result = (float)o;
                
                if (result < minValue)
                    return Rs.Add($"Value '{result}' is lower than allowed minimum value {minValue}.");

                return result;
            }
        });
    }
}