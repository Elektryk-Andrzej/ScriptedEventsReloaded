using System;
using System.Collections.Generic;
using System.Linq;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.FlagSystem.Structures;
using EventHandler = SER.ScriptSystem.EventSystem.EventHandler;

namespace SER.ScriptSystem.FlagSystem.Flags;

public class EventFlag : Flag
{
    public override FlagType Type => FlagType.Event;

    public override FlagArgument? InlineArgument => FlagArgument.EventName;

    public override Dictionary<FlagArgument, Func<string[], Result>> Arguments => new();
    
    public override Result TryBind(string[] inlineArgs)
    {
        switch (inlineArgs.Length)
        {
            case < 1:
                return "Event name is missing";
            case > 1:
                return "Too many arguments, only event name is allowed";
        }

        if (EventHandler.ConnectEvent(inlineArgs.First(), ScriptName).HasErrored(out var error))
        {
            return error;
        }
        
        return true;
    }

    public override void Confirm()
    {
    }

    public override void Unbind()
    {
        // done by event handler
    }
}