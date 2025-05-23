using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using UnityEngine;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a color argument used in a method.
/// </summary>
public class ColorArgument(string name) : BaseMethodArgument(name)
{
    public override OperatingValue Input => OperatingValue.Color;

    public override string AdditionalDescription =>
        "Colors should be provided in hexadecimal color string, that being '#RRGGBB' and '#RRGGBBAA', " +
        "which you can find on https://htmlcolorcodes.com. But you can also provide colors like 'red' or 'white', " +
        "and they will also work, similar to how <color=red> tag works in Unity.";

    public ArgumentEvaluation<Color> GetConvertSolution(BaseToken token)
    {
        return DefaultConvertSolution<Color>(token, null);
    }
}