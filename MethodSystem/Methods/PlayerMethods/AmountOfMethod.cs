using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class AmountOfMethod : TextReturningMethod, IPureMethod
{
    public override string Description => "Returns the amount of players in a given player variable.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("variable")
    ];

    public override void Execute()
    {
        TextReturn = Args.GetPlayers("variable").Count.ToString();
    }
}