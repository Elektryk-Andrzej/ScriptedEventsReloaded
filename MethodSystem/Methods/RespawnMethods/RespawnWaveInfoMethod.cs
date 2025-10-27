using System;
using LabApi.Features.Wrappers;
using PlayerRoles;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.ArgumentSystem.Structures;
using SER.Helpers.Exceptions;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.RespawnMethods;

public class RespawnWaveInfoMethod : LiteralValueReturningMethod, IReferenceResolvingMethod
{
    public Type ReferenceType => typeof(RespawnWave);

    public override Type[] LiteralReturnTypes => [typeof(NumberValue), typeof(TextValue)];

    public override string Description => null!;

    public override Argument[] ExpectedArguments { get; } =
    [
        new ReferenceArgument<RespawnWave>("respawnWave"),
        new OptionsArgument("property", 
            Option.Enum<Faction>(),
            "maxWaveSize",
            "respawnTokens",
            "influence",
            "secondsLeft")
    ];

    public override void Execute()
    {
        var wave = Args.GetReference<RespawnWave>("respawnWave");
        ReturnValue = Args.GetOption("property") switch
        {
            "faction" => new TextValue(wave.Faction.ToString()),
            "maxwavesize" => new NumberValue(wave.MaxWaveSize),
            "respawntokens" => new NumberValue(wave.RespawnTokens),
            "influence" => new NumberValue((decimal)wave.Influence),
            "secondsleft" => new NumberValue((decimal)wave.TimeLeft),
            _ => throw new AndrzejFuckedUpException()
        };
    }
}