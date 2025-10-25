using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.MethodSystem.BaseMethods;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.PlayerVariableMethods;

public class RemovePlayersMethod : ReturningMethod<PlayerValue>
{
    public override string Description => 
        "Returns players from the original variable that were not present in other variables.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("original players"),
        new PlayersArgument("players to remove")
        {
            ConsumesRemainingValues = true,
        }
    ];
    
    public override void Execute()
    {
        var originalPlayers = Args.GetPlayers("original players");
        var playersToRemove = Args
            .GetRemainingArguments<List<Player>, PlayersArgument>("players to remove")
            .Flatten();

        ReturnValue = new PlayerValue(originalPlayers.Where(p => !playersToRemove.Contains(p)));
    }
}