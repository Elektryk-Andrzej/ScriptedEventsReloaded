using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a number argument used in a method.
/// </summary>
public class NumberArgument(string name) : BaseMethodArgument(name)
{
    public override OperatingValue Input => OperatingValue.Float;
    public override string? AdditionalDescription => null;
    
    public ArgumentEvaluation<float> GetConvertSolution(BaseToken token)
    {
        return DefaultConvertSolution<float>(token, null!);
    }
}