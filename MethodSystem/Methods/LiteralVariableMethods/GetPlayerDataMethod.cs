using System;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Exceptions;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.LiteralVariableMethods;

public class GetPlayerDataMethod : ReturningMethod
{
    public override string Description => "Gets player data from the key.";

    public override Type[]? ReturnTypes => null;
    
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
            throw new ScriptRuntimeError($"Key '{key}' was not found for player '{player.Nickname}'.");
        }

        ReturnValue = value;
    }
}