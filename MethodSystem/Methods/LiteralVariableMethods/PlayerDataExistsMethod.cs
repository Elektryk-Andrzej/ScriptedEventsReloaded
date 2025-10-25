using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.LiteralVariableMethods;

public class PlayerDataExistsMethod : ReturningMethod<BoolValue>
{
    public override string Description => "Checks if a given key has an associated value for a given player.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayerArgument("player"),
        new TextArgument("key")
    ];
    
    public override void Execute()
    {
        var player = Args.GetPlayer("player");
        var key = Args.GetText("key");
        
        ReturnValue = new BoolValue(
            SetPlayerDataMethod.PlayerData.TryGetValue(player, out var dict) 
            && dict.ContainsKey(key)
        );
    }
}