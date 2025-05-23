using System;
using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.DoorMethods;

public class DoorInfoMethod : TextReturningMethod, IPureMethod
{
    public override string Description => "Returns information about the door.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new ReferenceArgument<Door>("door"),
        new OptionsArgument("info",
            "isOpen",
            "isClosed",
            "isLocked",
            "isUnlocked")
    ];
    
    public override void Execute()
    {
        var door = Args.GetReference<Door>("door");
        var info = Args.GetOption("info");
        
        TextReturn = info switch
        {
            "name" => door.DoorName.ToString(),
            "isopen" => door.IsOpened.ToString(),
            "isclosed" => (!door.IsOpened).ToString(),
            "islocked" => door.IsLocked.ToString(),
            "isunlocked" => (!door.IsLocked).ToString(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}