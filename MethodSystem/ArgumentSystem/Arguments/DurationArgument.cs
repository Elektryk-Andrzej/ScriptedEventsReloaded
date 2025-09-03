using System;
using JetBrains.Annotations;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a duration argument used in a method.
/// </summary>
public class DurationArgument(string name) : GenericMethodArgument(name)
{
    public override string AdditionalDescription => 
        "This argument supports a lot of formats, but you should use #ms (miliseconds), #s (seconds), #m (minutes), " +
        "#h (hours) and #d (days). You can replace # with a number like 21.37, 69 e.g. '.5s' for half a second. " +
        "Warning! You cannot put a negative number, as there is no negative duration (unless you bend spacetime).";
    
    [UsedImplicitly]
    public ArgumentEvaluation<TimeSpan> GetConvertSolution(BaseToken token)
    {
        return SingleSolutionConvert<TimeSpan>(token, OperatingValue.Duration);
    }
}