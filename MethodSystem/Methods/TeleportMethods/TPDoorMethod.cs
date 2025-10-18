using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.TeleportMethods;

// ReSharper disable once InconsistentNaming
public class TPDoorMethod : SynchronousMethod
{
    public override string Description => "Teleports players to a door.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players to teleport"),
        new DoorArgument("door to teleport to")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players to teleport");
        var door = Args.GetDoor("door to teleport to");
        
        players.ForEach(plr => 
            plr.Position = new(door.Position.x, door.Position.y + plr.Scale.y + 0.01f, door.Position.z));
    }
}