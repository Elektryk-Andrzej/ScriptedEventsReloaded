using System;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.PlayerVariableMethods;

public class GetPlayerFromReferenceMethod : ReturningMethod<PlayerValue>, IReferenceResolvingMethod
{
    public Type ReferenceType => typeof(Player);

    public override string Description => 
        "Returns a player variable with a single player from a reference.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new ReferenceArgument<Player>("playerReference")
    ];

    public override void Execute()
    {
        ReturnValue = new PlayerValue(Args.GetReference<Player>("playerReference"));
    }
}