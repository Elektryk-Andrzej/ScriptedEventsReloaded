using System;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.LiteralVariableMethods;

public class GetPlayerDataMethod : ReturningMethod, IAdditionalDescription
{
    public override string Description => "Gets player data from the key.";

    public override Type[] ReturnTypes => [typeof(TextValue)];
    
    public string AdditionalDescription =>
        "If the provided key has no associated value for this player, 'none' will be returned instead.";
    
    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayerArgument("player"),
        new TextArgument("key")
    ];

    public override void Execute()
    {
        var player = Args.GetPlayer("player");
        var key = Args.GetText("key");

        if (!SetPlayerDataMethod.PlayerData.TryGetValue(player, out var dict) || 
            !dict.TryGetValue(key, out var value))
        {
            Value = new TextValue("none");
            return;
        }

        Value = new TextValue(value);
    }
}