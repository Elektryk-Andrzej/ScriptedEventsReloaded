using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents an int amount argument used in a method.
/// <remarks>
/// This argument is specifically for *amount*. If you want to just get a number, use <see cref="NumberArgument"/> .
/// </remarks>
/// </summary>
public class IntAmountArgument(string name, int minValue) : BaseMethodArgument(name)
{
    public override OperatingValue Input => OperatingValue.Int;
    public override string AdditionalDescription => $"Value must be higher than {minValue} e.g. {minValue + 420}";
    
    public ArgumentEvaluation<int> GetConvertSolution(BaseToken token)
    {
        return DefaultConvertSolution<int>(token, new()
        {
            [OperatingValue.Int] = o =>
            {
                var result = (int)o;
                if (result < minValue)
                {
                    return Rs.Add($"Value {result} is lower than allowed minimum value {minValue}.");
                }

                return result;
            }
        });
    }
}