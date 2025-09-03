using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.LiteralVariableMethods;

public class GetPlayerDataMethod : TextReturningMethod, IPureMethod, IAdditionalDescription
{
    public override string Description => "Gets player data from the key.";

    public string AdditionalDescription =>
        "If the provided key has no associated value for this player, 'none' will be returned instead.";
    
    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayerArgument("player"),
        new TextArgument("key")
    ];

    public override void Execute()
    {
        var player = Args.GetSinglePlayer("player");
        var key = Args.GetText("key");

        if (!SetPlayerDataMethod.PlayerData.TryGetValue(player, out var dict) || 
            !dict.TryGetValue(key, out var value))
        {
            TextReturn = "none";
            return;
        }

        TextReturn = value;
    }
}