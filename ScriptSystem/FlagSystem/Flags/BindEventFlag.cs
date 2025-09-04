using System;
using System.Collections.Generic;
using System.Linq;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.FlagSystem.Structures;
using EventHandler = SER.ScriptSystem.EventSystem.EventHandler;

namespace SER.ScriptSystem.FlagSystem.Flags;

public class BindEventFlag : Flag
{
    public override string Description =>
        "Binds a script to an in-game event. When the event happens, the script will execute. " +
        "Events can sometimes also carry information of their own, ";

    public override (string argName, string description)? InlineArgDescription =>
        ("eventName", "The name of the event to bind to.");
    
    public override Dictionary<string, (string description, Func<string[], Result> handler)> Arguments => new();

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