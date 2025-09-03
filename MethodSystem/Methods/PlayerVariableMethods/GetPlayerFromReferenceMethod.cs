using System;
using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.PlayerVariableMethods;

public class GetPlayerFromReferenceMethod : PlayerReturningMethod, IReferenceResolvingMethod
{
    public Type ReferenceType => typeof(Player);

    public override string Description => 
        "Returns a player variable with a single player from a reference.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new ReferenceArgument<Player>("playerReference")
    ];

    public override void Execute()
    {
        PlayerReturn = [Args.GetReference<Player>("playerReference")];
    }
}