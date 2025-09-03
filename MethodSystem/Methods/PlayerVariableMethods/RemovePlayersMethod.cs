using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using SER.Helpers.Extensions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerVariableMethods;

public class RemovePlayersMethod : PlayerReturningMethod
{
    public override string Description => 
        "Returns players from the original variable that were not present in other variables.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
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

        PlayerReturn = originalPlayers.Where(p => !playersToRemove.Contains(p)).ToArray();
    }
}