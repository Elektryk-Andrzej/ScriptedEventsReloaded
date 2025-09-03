using System;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;
using SER.Helpers.Exceptions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;
using SER.VariableSystem;

namespace SER.MethodSystem.Methods.HealthMethods;

public class DamageInfoMethod : ReferenceResolvingMethod, IAdditionalDescription
{
    public override Type ReferenceType => typeof(DamageHandlerBase);

    public override GenericMethodArgument[] ExpectedArguments { get; } =
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
        
        TextReturn = Args.GetOption("property") switch
        {
            "damage" => standard?.Damage.ToString() ?? "none",
            "hitbox" => standard?.Hitbox.ToString() ?? "none",
            "firearmused" => firearm?.Firearm is not null
                ? ObjectReferenceSystem.RegisterObject(firearm.Firearm)
                : "none",
            "attacker" => attacker?.Attacker is not null
                ? ObjectReferenceSystem.RegisterObject(attacker.Attacker)
                : "none",
            _ => throw new AndrzejFuckedUpException("out of range")
        };
    }

    public string AdditionalDescription =>
        "A lot of options here might not be available depending on which DamageHandler is used in game. " +
        "It's advised you check every accessed value for 'none' before using it.";
}