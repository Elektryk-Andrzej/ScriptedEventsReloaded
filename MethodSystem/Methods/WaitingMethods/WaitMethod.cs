using System.Collections.Generic;
using MEC;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.WaitingMethods;

public class WaitMethod : YieldingMethod
{
    public override string Description => "Halts execution of the script for a specified amount of time.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new DurationArgument("duration")
    ];

    public override IEnumerator<float> Execute()
    {
        var dur = Args.GetDuration("duration");
        yield return Timing.WaitForSeconds((float)dur.TotalSeconds);
    }
}