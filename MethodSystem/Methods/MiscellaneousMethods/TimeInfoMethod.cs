using System;
using SER.Helpers.Exceptions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.MiscellaneousMethods;

public class TimeInfoMethod : TextReturningMethod, IPureMethod
{
    public override string Description => "Returns information about current time.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new OptionsArgument("options",
            "second",
            "minute",
            "hour",
            "year",
            "dayOfWeek",
            new("dayOfWeekNumber", "Instead of returning e.g. 'Monday', will return 1"),
            "dayOfMonth",
            "dayOfYear")
    ];
    
    public override void Execute()
    {
        TextReturn = Args.GetOption("options").ToLower() switch
        {
            "second" => DateTime.Now.Second.ToString(),
            "minute" => DateTime.Now.Minute.ToString(),
            "hour" => DateTime.Now.Hour.ToString(),
            "year" => DateTime.Now.Year.ToString(),
            "dayofweek" => DateTime.Now.DayOfWeek.ToString(),
            "dayofweeknumber" => (int)DateTime.Now.DayOfWeek == 0
                ? "7"
                : ((int)DateTime.Now.DayOfWeek).ToString(),
            "dayofmonth" => DateTime.Now.Day.ToString(),
            "dayofyear" => DateTime.Now.DayOfYear.ToString(),
            _ => throw new DeveloperFuckupException()
        };
    }
}