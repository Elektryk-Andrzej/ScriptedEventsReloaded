﻿using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.DoorMethods;

public class CloseDoorMethod : Method
{
    public override string Description => "Closes doors.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [       
        new DoorsArgument("doors")
    ];
    
    public override void Execute()
    {
        var doors = Args.GetDoors("doors");;
        
        foreach (var door in doors)
        {
            door.IsOpened = false;
        }
    }
}