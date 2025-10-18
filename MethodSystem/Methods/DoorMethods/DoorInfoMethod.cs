using System;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.ArgumentSystem.Structures;
using SER.Helpers.Exceptions;
using SER.MethodSystem.BaseMethods;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.DoorMethods;

public class DoorInfoMethod : ReferenceResolvingMethod
{
    public override Type ReferenceType => typeof(Door);
    public override Type[]? ReturnTypes => [typeof(TextValue), typeof(BoolValue)];

    public override Argument[] ExpectedArguments { get; } =
    [
        new ReferenceArgument<Door>("door"),
        new OptionsArgument("info",
            "isOpen",
            "isClosed",
            "isLocked",
            "isUnlocked",
            Option.Enum<DoorName>(),
            "unityName")
    ];

    public override void Execute()
    {
        var door = Args.GetReference<Door>("door");
        var info = Args.GetOption("info");
        
        Value = info switch
        {
            "doorname" => new TextValue(door.DoorName.ToString()),
            "unityname" => new TextValue(door.Base.name),
            "isopen" => new BoolValue(door.IsOpened),
            "isclosed" => new BoolValue(!door.IsOpened),
            "islocked" => new BoolValue(door.IsLocked),
            "isunlocked" => new BoolValue(!door.IsLocked),
            _ => throw new AndrzejFuckedUpException()
        };
    }
}