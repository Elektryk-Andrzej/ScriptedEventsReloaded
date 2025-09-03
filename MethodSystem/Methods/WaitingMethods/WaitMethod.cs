﻿using System.Collections.Generic;
using MEC;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.WaitingMethods;

public class WaitMethod : YieldingMethod
{
    public override string Description => "Halts execution of the script for a specified amount of time.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new DurationArgument("duration")
    ];

    public override IEnumerator<float> Execute()
    {
        var dur = Args.GetDuration("duration");
        yield return Timing.WaitForSeconds((float)dur.TotalSeconds);
    }
}