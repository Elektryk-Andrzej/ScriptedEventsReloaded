using JetBrains.Annotations;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using UnityEngine;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a color argument used in a method.
/// </summary>
public class ColorArgument(string name) : GenericMethodArgument(name)
{
    public override string AdditionalDescription =>
        "Colors must be provided in hexadecimal color format, that being '#RRGGBB' or '#RRGGBBAA'.";

    [UsedImplicitly]
    public ArgumentEvaluation<Color> GetConvertSolution(BaseToken token)
    {
        return SingleSolutionConvert<Color>(token, OperatingValue.Color);
    }
}