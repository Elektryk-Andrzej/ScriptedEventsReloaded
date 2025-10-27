using System;
using System.Linq;
using JetBrains.Annotations;
using LabApi.Features;
using LabApi.Features.Console;
using MEC;
using SER.FlagSystem.Structures;
using SER.Helpers.Extensions;
using SER.MethodSystem;
using SER.MethodSystem.Methods.LiteralVariableMethods;
using SER.ScriptSystem;
using SER.VariableSystem;
using EventHandler = SER.EventSystem.EventHandler;
using Events = LabApi.Events.Handlers;

namespace SER.Plugin;

[UsedImplicitly]
public class MainPlugin : LabApi.Loader.Features.Plugins.Plugin
{
    public override string Name => "SER";
    public override string Description => "The scripting language for SCP:SL.";
    public override string Author => "Elektryk_Andrzej";
    public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;
    public override Version Version => new(0, 3, 0);
    
    public static string GitHubLink => "https://github.com/ScriptedEvents/ScriptedEventsReloaded";
    public static string HelpCommandName => "serhelp";
    public static MainPlugin Instance { get; private set; } = null!;

    public record struct Contributor(string Name, Contribution Contribution);

    [Flags]
    public enum Contribution
    {
        None             = 0,
        Developer        = 1 << 1,
        QualityAssurance = 1 << 2,
        Sponsor          = 1 << 3,
        Betatester       = 1 << 4,
        EarlyAdopter     = 1 << 5
    }
    
    public static Contributor[] Contributors => 
    [
        new(Instance.Author, Contribution.Developer),
        new("Whitty985playz", Contribution.QualityAssurance | Contribution.EarlyAdopter),
        new("Jraylor", Contribution.Sponsor),
        new("Luke", Contribution.Sponsor),
        new("Krzysiu Wojownik", Contribution.QualityAssurance),
        new("Raging Tornado", Contribution.Betatester)
    ]; 
    
    public override void Enable()
    {
        Instance = this;
        
        Script.StopAll();
        EventHandler.Initialize();
        MethodIndex.Initialize();
        VariableIndex.Initialize();
        Flag.RegisterFlags();
        
        Events.ServerEvents.WaitingForPlayers += OnServerFullyInit;
        Events.ServerEvents.RoundRestarted += () =>
        {
            Script.StopAll();
            SetPlayerDataMethod.PlayerData.Clear();
        };
        
        Timing.CallDelayed(1.5f, FileSystem.Initialize);
        Logger.Raw(
            """
             #####################################
               █████████  ██████████ ███████████  
              ███░░░░░███░░███░░░░░█░░███░░░░░███ 
             ░███    ░░░  ░███  █ ░  ░███    ░███ 
             ░░█████████  ░██████    ░██████████  
              ░░░░░░░░███ ░███░░█    ░███░░░░░███ 
              ███    ░███ ░███ ░   █ ░███    ░███ 
             ░░█████████  ██████████ █████   █████
              ░░░░░░░░░  ░░░░░░░░░░ ░░░░░   ░░░░░ 
             #####################################
             
             This project would not be possible without the help of:
             
             """ + Contributors
                .Select(c => $"> {c.Name} as {c
                    .Contribution
                    .GetFlags()
                    .Select(f => f.ToString().Spaceify())
                    .JoinStrings(", ")}"
                )
                .JoinStrings("\n"),
            ConsoleColor.Cyan
        );
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

             Help command: {HelpCommandName}
             GitHub repository: {GitHubLink}
             """,
            ConsoleColor.Cyan
        );
    }
}