using System;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class AmountOfMethod : ReturningMethod
{
    public override string Description => "Returns the amount of players in a given player variable.";

    public override Type[] ReturnTypes => [typeof(NumberValue)];
    
    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("variable")
    ];

    public override void Execute()
    {
        Value = new NumberValue(Args.GetPlayers("variable").Count);
    }
}