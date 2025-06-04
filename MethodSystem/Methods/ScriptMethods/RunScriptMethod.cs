using System.Collections.Generic;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.VariableSystem.Structures;

namespace SER.MethodSystem.Methods.ScriptMethods;

public class RunScriptMethod : Method
{
    public override string Description => "Runs a script.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new ScriptArgument("script"),
        new VariableArgument("variablesToPass")
        {
            ConsumesRemainingValues = true,
            DefaultValue = new List<IVariable>(),
            Description = "Passes an exact copy of the provided variables to the script."
        }
    ];
    
    public override void Execute()
    {
        var script = Args.GetScript("script");
        var variables = Args.GetRemainingArguments<IVariable, VariableArgument>("variablesToPass");
        
        foreach (var var in variables)
        {
            switch (var)
            {
                case PlayerVariable pvar:
                    script.AddLocalPlayerVariable(pvar);
                    continue;
                case LiteralVariable lvar:
                    script.AddLocalLiteralVariable(lvar);
                    continue;
            }
        }

        script.Run();
    }
}