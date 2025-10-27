using System;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Exceptions;
using SER.MethodSystem.BaseMethods;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.TimeMethods;

public class ToDurationMethod : ReturningMethod<DurationValue>
{
    public enum DurationUnit
    {
        Milliseconds,
        Seconds,
        Minutes,
        Hours,
    }
    
    public override string Description => "Creates a duration value from a number and a unit. Used when a number is not known ahead of time.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new FloatArgument("length", 0),
        new EnumArgument<DurationUnit>("unit")
    ];
    
    public override void Execute()
    {
        var length = Args.GetFloat("length");
        var unit = Args.GetEnum<DurationUnit>("unit");
        ReturnValue = unit switch
        {
            DurationUnit.Milliseconds => TimeSpan.FromMilliseconds(length),
            DurationUnit.Seconds => TimeSpan.FromSeconds(length),
            DurationUnit.Minutes => TimeSpan.FromMinutes(length),
            DurationUnit.Hours => TimeSpan.FromHours(length),
            _ => throw new AndrzejFuckedUpException()
        };
    }
}