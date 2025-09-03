using System;
using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.DoorMethods;

public class DoorInfoMethod : ReferenceResolvingMethod
{
    public override Type ReferenceType => typeof(Door);

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new ReferenceArgument<Door>("door"),
        new OptionsArgument("info",
            "isOpen",
            "isClosed",
            "isLocked",
            "isUnlocked",
            "name",
            "unityName")
    ];

    public override void Execute()
    {
        var door = Args.GetReference<Door>("door");
        var info = Args.GetOption("info");
        
        TextReturn = info switch
        {
            "name" => door.DoorName.ToString(),
            "unityname" => door.Base.name,
            "isopen" => door.IsOpened.ToString(),
            "isclosed" => (!door.IsOpened).ToString(),
            "islocked" => door.IsLocked.ToString(),
            "isunlocked" => (!door.IsLocked).ToString(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}