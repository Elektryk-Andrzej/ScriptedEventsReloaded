using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerVariableMethods;

public class LimitPlayersMethod : PlayerReturningMethod
{
    public override string Description =>
        "Returns a player variable with amount of players being equal or lower than the provided amount.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new IntArgument("amount", 1)   
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var amount = Args.GetIntAmount("amount");

        while (amount > players.Len && players.Len > 0)
        {
            players.PullRandomItem();
        }
        
        PlayerReturn = players.ToArray();
    }
}