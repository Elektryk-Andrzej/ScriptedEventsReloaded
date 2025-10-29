using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SER.FlagSystem.Structures;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.MethodSystem.Methods.ScriptMethods;

namespace SER.FlagSystem.Flags;

[UsedImplicitly]
public class OnCustomTriggerFlag : Flag
{
    private string _trigger = null!;
    
    public static readonly Dictionary<string, List<string>> ScriptsBoundToTrigger = [];
    
    public override string Description =>
        $"Makes a script execute when a trigger with a matching name is fired (done using {nameof(TriggerMethod).Replace("Method", "")} method)";

    public override Argument? InlineArgument => new(
        "triggerName",
        "The name of the trigger to bind to.",
        inlineArgs =>
        {
            switch (inlineArgs.Length)
            {
                case < 1: return "Trigger name is missing";
                case > 1: return "Too many arguments, only trigger name is allowed";
            }

            _trigger = inlineArgs.First();
            ScriptsBoundToTrigger.AddOrInitListWithKey(_trigger, ScriptName);
            return true;
        },
        true
    );
    
    public override Argument[] Arguments => [];

    public override void FinalizeFlag()
    {
    }

    public override void Unbind()
    {
        if (ScriptsBoundToTrigger.TryGetValue(_trigger, out var list))
        {
            list.Remove(ScriptName);
        }
    }
}