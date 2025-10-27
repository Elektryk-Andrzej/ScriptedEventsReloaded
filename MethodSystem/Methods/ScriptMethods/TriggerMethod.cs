using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.FlagSystem.Flags;
using SER.Helpers.Exceptions;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem.Structures;

namespace SER.MethodSystem.Methods.ScriptMethods;

public class TriggerMethod : SynchronousMethod
{
    public override string? Description => "Fires a given trigger, executing scripts which are attached to it.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new TextArgument("trigger name")
    ];
    
    public override void Execute()
    {
        if (!OnCustomTriggerFlag.ScriptsBoundToTrigger.TryGetValue(Args.GetText("trigger name"), out var scripts))
        {
            return;
        }

        foreach (var scriptName in scripts)
        {
            if (ScriptSystem.Script.CreateByScriptName(scriptName, ScriptExecutor.Get()).HasErrored(out var error, out var script))
            {
                throw new ScriptRuntimeError(error);
            }
            
            script.Run();
        }
    }
}