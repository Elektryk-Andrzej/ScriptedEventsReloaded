using JetBrains.Annotations;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a boolean argument used in a method.
/// </summary>
public class BoolArgument(string name) : GenericMethodArgument(name)
{
    public override string? AdditionalDescription => null;

    [UsedImplicitly]
    public ArgumentEvaluation<bool> GetConvertSolution(BaseToken token)
    {
        return SingleSolutionConvert<bool>(token, OperatingValue.Boolean);
    }
}