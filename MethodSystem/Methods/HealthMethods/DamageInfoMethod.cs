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

public class DamageInfoMethod : ReturningMethod, IReferenceResolvingMethod, IAdditionalDescription
{
    public Type ReferenceType => typeof(DamageHandlerBase);
    public override Type[] ReturnTypes => [typeof(TextValue), typeof(ReferenceValue)];

    public override string Description => null!;

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
        
        ReturnValue = Args.GetOption("property") switch
        {
            "damage" => new TextValue(standard?.Damage.ToString() ?? "none"),
            "hitbox" => new TextValue(standard?.Hitbox.ToString() ?? "none"),
            "firearmused" => new ReferenceValue(firearm?.Firearm),
            "attacker" => new ReferenceValue(attacker?.Attacker),
            _ => throw new AndrzejFuckedUpException("out of range")
        };
    }

    public string AdditionalDescription =>
        "A lot of options here might not be available depending on which DamageHandler is used in game. " +
        "It's advised you check every accessed value for 'none' before using it.";
}