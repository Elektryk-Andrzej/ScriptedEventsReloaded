using System;
using LabApi.Features.Wrappers;
using SER.Helpers.Exceptions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.RespawnMethods;

public class RespawnWaveInfoMethod : ReferenceResolvingMethod
{
    public override Type ReferenceType => typeof(RespawnWave);

    public override GenericMethodArgument[] ExpectedArguments { get; } =
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
            _ => throw new AndrzejFuckedUpException()
        };
    }
}