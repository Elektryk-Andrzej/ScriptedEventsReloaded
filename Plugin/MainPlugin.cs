using System;
using JetBrains.Annotations;
using LabApi.Features;
using LabApi.Features.Console;
using SER.MethodSystem;
using SER.MethodSystem.Methods.LiteralVariableMethods;
using SER.ScriptSystem;
using SER.ScriptSystem.FlagSystem.Structures;
using SER.VariableSystem;
using EventHandler = SER.ScriptSystem.EventSystem.EventHandler;
using Events = LabApi.Events.Handlers;

namespace SER.Plugin;

[UsedImplicitly]
public class MainPlugin : LabApi.Loader.Features.Plugins.Plugin
{
    public override string Name => "SER";
    public override string Description => "The scripting language for SCP:SL.";
    public override string Author => "Elektryk_Andrzej";
    public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;
    public override Version Version => new(0, 1, 0);
    
    public static string GitHubLink => "https://github.com/Elektryk-Andrzej/ScriptedEventsReloaded";
    public static string WikiLink => GitHubLink + "/wiki";
    public static MainPlugin Instance { get; private set; } = null!;
    
    public override void Enable()
    {
        Instance = this;
        
        Script.StopAll();
        EventHandler.Initialize();
        MethodIndex.Initialize();
        PlayerVariableIndex.Initialize();
        Flag.RegisterFlags();
        FileSystem.Initialize();
        
        Events.ServerEvents.WaitingForPlayers += OnServerFullyInit;
        Events.ServerEvents.RoundRestarted += () =>
        {
            Script.StopAll();
            SetPlayerDataMethod.PlayerData.Clear();
        };
    }

    public override void Disable()
    {
        Script.StopAll();
    }
    
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