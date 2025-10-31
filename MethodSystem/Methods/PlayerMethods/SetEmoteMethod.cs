using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using System.Collections.Generic;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class SetEmoteMethod : SynchronousMethod
{
    public override string Description => "Sets emotion for specified players";

    public override Argument[] ExpectedArguments => 
    [
        new PlayersArgument("players"),
        new EnumArgument<EmotionPresetType>("emotion")
    ];

    public override void Execute()
    {
        List<Player> players = Args.GetPlayers("players");
        EmotionPresetType emotion = Args.GetEnum<EmotionPresetType>("emotion");
        
        players.ForEach(p => p.Emotion = emotion);
    }
}
