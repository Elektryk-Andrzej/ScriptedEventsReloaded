using System.Collections.Generic;
using MEC;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.VariableSystem.Bases;

namespace SER.MethodSystem.Methods.ScriptMethods;

public class RunScriptAndWaitMethod : YieldingMethod
{
    public override string Description => "Runs a script and waits until the ran script has finished.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new ScriptArgument("script"),
        new VariableArgument("variablesToPass")
        {
            ConsumesRemainingValues = true,
            DefaultValue = new List<Variable>(),
            Description = "Passes an exact copy of the provided variables to the script."
        }
    ];
    
    public override IEnumerator<float> Execute()
    {
        var script = Args.GetScript("script");
        var variables = Args.GetRemainingArguments<Variable, VariableArgument>("variablesToPass");
        
        script.AddVariables(variables);
        script.Run();
        
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        while (script is not null && script.IsRunning)
        {
            yield return Timing.WaitForOneFrame;
        }
    }
}