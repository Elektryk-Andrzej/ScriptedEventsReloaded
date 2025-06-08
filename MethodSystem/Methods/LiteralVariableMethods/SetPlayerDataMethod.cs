using System.Collections.Generic;
using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.LiteralVariableMethods;

// ReSharper disable once ClassNeverInstantiated.Global
public class SetPlayerDataMethod : Method, IAdditionalDescription
{
    public static readonly Dictionary<Player, Dictionary<string, string>> PlayerData = [];
    
    public override string Description => "Associates a custom key with a value for a given player.";
    
    public string AdditionalDescription =>
        "This method ties a specific value to a specific player, allowing you to e.g. keep track of how many seconds " +
        "each player was on surface zone, as you're tying the amount of time to a specific player, independently of " +
        "other players. For this, you would create a key e.g. 'surfaceTime', and under that key you can start saving " +
        "the value. It's basically a dictionary/hashmap attached to a player.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new SinglePlayerArgument("player"),
        new TextArgument("key"),
        new TextArgument("valueToSet")
    ];

    public override void Execute()
    {
        var player = Args.GetSinglePlayer("player");
        var key = Args.GetText("key");
        var valueToSet = Args.GetText("valueToSet");
        
        if (PlayerData.TryGetValue(player, out var dict))
        {
            dict[key] = valueToSet;
        }
        else
        {
            PlayerData.Add(player, new Dictionary<string, string> { { key, valueToSet } });
        }
    }
}