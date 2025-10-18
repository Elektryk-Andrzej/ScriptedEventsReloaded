using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerVariableMethods;

public class JoinPlayersMethod : PlayerReturningMethod
{
    public override string Description =>
        "Returns all players that were provided from multiple player variables.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players")
        {
            ConsumesRemainingValues = true
        }
    ];
    
    public override void Execute()
    {
        PlayerReturn = Args
            .GetRemainingArguments<List<Player>, PlayersArgument>("players")
            .Flatten()
            .Distinct()
            .ToArray();
    }
}