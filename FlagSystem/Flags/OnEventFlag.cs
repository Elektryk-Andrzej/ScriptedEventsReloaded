﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SER.FlagSystem.Structures;
using SER.Helpers.ResultSystem;
using EventHandler = SER.EventSystem.EventHandler;

namespace SER.FlagSystem.Flags;

[UsedImplicitly]
public class OnEventFlag : Flag
{
    public override string Description =>
        "Binds a script to an in-game event. When the event happens, the script will execute. " +
        "Events can sometimes also carry information of their own, ";

    public override Argument? InlineArgument => new(
        "eventName",
        "The name of the event to bind to.",
        inlineArgs =>
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
        },
        true
    );
    
    public override Argument[] Arguments => [];
    
    public override void FinalizeFlag()
    {
    }

    public override void Unbind()
    {
        // done by event handler
    }
}