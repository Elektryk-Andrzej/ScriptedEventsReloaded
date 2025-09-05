using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.VariableSystem.Structures;

namespace SER.MethodSystem.Methods.ScriptMethods;

public class TransferVariablesMethod : SynchronousMethod
{
    public override string Description => "Makes a copy of the given local variable(s) in a different script.";

    public override GenericMethodArgument[] ExpectedArguments =>
    [
        new ScriptArgument("target script"),
        new VariableArgument("variables")
        {
            ConsumesRemainingValues = true,
        }
    ];

    public override void Execute()
    {
        var script = Args.GetScript("target script");
        var variables = Args.GetRemainingArguments<IVariable, VariableArgument>("variables");
        
        script.AddVariables(variables);
    }
}