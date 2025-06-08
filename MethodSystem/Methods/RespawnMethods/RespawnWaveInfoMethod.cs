using LabApi.Features.Wrappers;
using SER.Helpers.Exceptions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.RespawnMethods;

public class RespawnWaveInfoMethod : TextReturningMethod, IPureMethod
{
    public override string Description => "Returns information about a given respawn wave reference.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new ReferenceArgument<RespawnWave>("respawnWave"),
        new OptionsArgument("property",
            "faction",
            "maxWaveSize",
            "respawnTokens",
            "influence",
            "secondsLeft")
    ];
    
    public override void Execute()
    {
        var wave = Args.GetReference<RespawnWave>("respawnWave");
        TextReturn = Args.GetOption("property") switch
        {
            "faction" => wave.Faction.ToString(),
            "maxwavesize" => wave.MaxWaveSize.ToString(),
            "respawntokens" => wave.RespawnTokens.ToString(),
            "influence" => wave.Influence.ToString(),
            "secondsleft" => wave.TimeLeft.ToString(),
            _ => throw new DeveloperFuckupException()
        };
    }
}