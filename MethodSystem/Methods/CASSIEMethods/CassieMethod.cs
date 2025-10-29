using System;
using System.Linq;
using Respawning;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.CASSIEMethods;

public class CassieMethod : SynchronousMethod
{
    public override string Description => "Makes a CASSIE announcement.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new OptionsArgument("mode",
            "jingle",
            "noJingle"
        ),
        new TextArgument("message"),
        new TextArgument("translation")
        {
            DefaultValue = "",
        },
        new BoolArgument("should glitch")
        {
            Description = "If true, SER will add random glitch effects to the announcement.",
            DefaultValue = false,
        }
    ];
    
    public override void Execute()
    {
        var isNoisy = Args.GetOption("mode") == "jingle";
        var message = Args.GetText("message");
        var translation = Args.GetText("translation");
        var glitch = Args.GetBool("should glitch");

        if (glitch)
        {
            message = "jam_100_9 " + message
                .Split([' '], StringSplitOptions.RemoveEmptyEntries)
                .Select(part =>
                {
                    if (UnityEngine.Random.Range(1, 3) == 1) return part;
                    return $"pitch_{UnityEngine.Random.Range(0.7f, 1.2f)} {part}";

                })
                .JoinStrings(" ");
        }

        if (string.IsNullOrEmpty(translation))
        {
            RespawnEffectsController.PlayCassieAnnouncement(
                message, 
                false, 
                isNoisy
            );
        }
        else
        {
            RespawnEffectsController.PlayCassieAnnouncement(
                message, 
                false, 
                isNoisy, 
                true,
                translation
            );
        }
    }
}