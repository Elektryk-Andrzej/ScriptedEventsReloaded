using System.Collections.Generic;
using MEC;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.WaitingMethods;

public class WaitUntilMethod : YieldingMethod
{
    public override string Description => "Halts execution of the script until the given condition is true.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new BoolArgument("condition")
    ];

    public override IEnumerator<float> Execute()
    {
        var condFunc = Args.GetBoolFunc("condition");
        while (!condFunc()) yield return Timing.WaitForOneFrame;
    }
}