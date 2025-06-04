using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using SER.Helpers.Extensions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.PlayerVariableMethods;

public class JoinPlayersMethod : PlayerReturningMethod, IPureMethod
{
    public override string Description =>
        "Returns all players that were provided from multiple player variables.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
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