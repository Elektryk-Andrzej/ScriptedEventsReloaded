using System;
using System.Collections.Generic;
using LabApi.Features;
using LabApi.Features.Console;
using SER.MethodSystem;
using SER.ScriptSystem;
using SER.VariableSystem;
using Events = LabApi.Events.Handlers;

namespace SER.Plugin;

public class MainPlugin : LabApi.Loader.Features.Plugins.Plugin
{
    public override string Name => "SER";
    public override string Description => "The scripting language for SCP:SL.";
    public override string Author => "Elektryk_Andrzej";
    public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;
    public override Version Version => new(0, 1, 0);
    public static string GitHubLink => "https://github.com/Elektryk-Andrzej/ScriptedEventsReloaded";
    public static string WikiLink => GitHubLink + "/wiki";
    
    public override void Enable()
    {
        ScriptFlagHandler.Initialize();
        MethodIndex.Initalize();
        PlayerVariableIndex.Initalize();
        FileSystem.Initalize();
        
        Events.ServerEvents.WaitingForPlayers += OnServerFullyInit;
    }

    public override void Disable()
    {
        foreach (var script in RunningScripts)
        {
            script.Stop();
        }
    }

    public static readonly List<Script> RunningScripts = [];

    private void OnServerFullyInit()
    {
        Logger.Raw(
            $"""
                    Thank you for using ### Scripted Events Reloaded ### by {Author}!

                      █████████  ██████████ ███████████  
                     ███░░░░░███░░███░░░░░█░░███░░░░░███ 
                    ░███    ░░░  ░███  █ ░  ░███    ░███ 
                    ░░█████████  ░██████    ░██████████  
                     ░░░░░░░░███ ░███░░█    ░███░░░░░███ 
                     ███    ░███ ░███ ░   █ ░███    ░███ 
                    ░░█████████  ██████████ █████   █████
                     ░░░░░░░░░  ░░░░░░░░░░ ░░░░░   ░░░░░ 
                     
                    GitHub repository: {GitHubLink}
                    Wiki page: {WikiLink}
                    """,
            ConsoleColor.Yellow);
    }
}