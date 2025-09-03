using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.TeleportMethods;

// ReSharper disable once InconsistentNaming
public class TPRoomMethod : SynchronousMethod
{
    public override string Description => "Teleports players to a room.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players to teleport"),
        new RoomArgument("room to teleport to")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players to teleport");
        var room = Args.GetSingleRoom("room to teleport to");
        
        players.ForEach(plr => 
            plr.Position = new(room.Position.x, room.Position.y + plr.Scale.y + 0.01f, room.Position.z));
    }
}