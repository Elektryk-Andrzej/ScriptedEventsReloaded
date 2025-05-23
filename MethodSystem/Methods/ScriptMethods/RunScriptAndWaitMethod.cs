using System.Collections.Generic;
using MEC;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.VariableSystem.Structures;

namespace SER.MethodSystem.Methods.ScriptMethods;

public class RunScriptAndWaitMethod : YieldingMethod
{
    public override string Description => "Runs a script and waits until the ran script has finished.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new ScriptArgument("script"),
        new VariableArgument("variablesToPass")
        {
            ConsumesRemainingArguments = true,
            DefaultValue = new List<IVariable>(),
            Description = "Passes an exact copy of the provided variables to the script."
        }
    ];
    
    public override IEnumerator<float> Execute()
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
        while (script.IsRunning)
        {
            yield return Timing.WaitForOneFrame;
        }
    }
}