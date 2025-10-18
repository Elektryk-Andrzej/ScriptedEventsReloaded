using System;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.ArgumentSystem.Structures;
using SER.Helpers.Exceptions;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.HealthMethods;

public class DamageInfoMethod : ReferenceResolvingMethod, IAdditionalDescription
{
    public override Type ReferenceType => typeof(DamageHandlerBase);
    public override Type[]? ReturnTypes => [typeof(TextValue), typeof(ReferenceValue)];

    public override Argument[] ExpectedArguments { get; } =
    [
        new ReferenceArgument<DamageHandlerBase>("handler"),
        new OptionsArgument("property",
            "damage",
            Option.Enum<HitboxType>("hitbox"), 
            Option.Reference<Item>("firearmUsed"), 
            Option.Reference<Player>("attacker"))
    ];

    public override void Execute()
    {
        var handler = Args.GetReference<DamageHandlerBase>("handler");
        var standard = handler as StandardDamageHandler;
        var firearm = handler as FirearmDamageHandler;
        var attacker = handler as AttackerDamageHandler;
        
        Value = Args.GetOption("property") switch
        {
            "damage" => new TextValue(standard?.Damage.ToString() ?? "none"),
            "hitbox" => new TextValue(standard?.Hitbox.ToString() ?? "none"),
            "firearmused" => firearm?.Firearm is not null
                ? new ReferenceValue(firearm.Firearm)
                : new TextValue("none"),
            "attacker" => attacker?.Attacker is not null
                ? new ReferenceValue(attacker.Attacker)
                : new TextValue("none"),
            _ => throw new AndrzejFuckedUpException("out of range")
        };
    }

    public string AdditionalDescription =>
        "A lot of options here might not be available depending on which DamageHandler is used in game. " +
        "It's advised you check every accessed value for 'none' before using it.";
}