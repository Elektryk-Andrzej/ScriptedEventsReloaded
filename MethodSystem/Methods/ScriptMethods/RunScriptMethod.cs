using System.Collections.Generic;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.VariableSystem.Bases;

namespace SER.MethodSystem.Methods.ScriptMethods;

public class RunScriptMethod : SynchronousMethod
{
    public override string Description => "Runs a script.";

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
    
    public override void Execute()
    {
        var script = Args.GetScript("script");
        var variables = Args.GetRemainingArguments<Variable, VariableArgument>("variablesToPass");
        
        script.AddVariables(variables);
        script.Run();
    }
}