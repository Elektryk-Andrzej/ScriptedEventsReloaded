using PlayerRoles;
using PlayerRoles.FirstPersonControl.Spawnpoints;
using SER.Helpers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.TeleportMethods;

// ReSharper disable once InconsistentNaming
public class TPSpawnMethod : SynchronousMethod
{
    public override string Description => "Teleports players to where a specified role would spawn.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players to teleport"),
        new EnumArgument<RoleTypeId>("spawnpoint of role")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players to teleport");
        var role = Args.GetEnum<RoleTypeId>("spawnpoint of role");

        if (!RoleSpawnpointManager.TryGetSpawnpointForRole(role, out var spawnpoint) 
            || !spawnpoint.TryGetSpawnpoint(out var position, out _))
        {
            Log.Error(Script, $"Role {role} doesn't have a defined spawnpoint.");
            return;
        }
        
        players.ForEach(plr => plr.Position = position);
    }
}