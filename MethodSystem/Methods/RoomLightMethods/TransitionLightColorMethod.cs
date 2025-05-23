using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MEC;
using SER.Helpers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using UnityEngine;

namespace SER.MethodSystem.Methods.RoomLightMethods;

public class TransitionLightColorMethod : Method
{
    public override string Description => "Transitions smoothly the light color for rooms.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new RoomsArgument("rooms"),
        new ColorArgument("color"),
        new DurationArgument("transitionDuration")
    ];
    
    public override void Execute()
    {
        var rooms = Args.GetRooms("rooms");
        var targetColor = Args.GetColor("color");
        var transitionDuration = Args.GetDuration("transitionDuration");
        
        foreach (var room in rooms)
        {
            TransitionColor(room, targetColor, transitionDuration.ToFloatSeconds()).Run(Script);
        }
    }
    
    private static IEnumerator<float> TransitionColor(Room room, Color targetColor, float duration)
    {
        Color startColor = room.LightController?.OverrideLightsColor ?? Color.clear;
        if (startColor == targetColor || startColor == Color.clear) yield break;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            
            Color currentColor = Color.Lerp(startColor, targetColor, t);
            room.AllLightControllers.ForEachItem(ctrl => ctrl.OverrideLightsColor = currentColor);
            
            yield return Timing.WaitForOneFrame;
        }
        
        room.AllLightControllers.ForEachItem(ctrl => ctrl.OverrideLightsColor = targetColor);
    }
}