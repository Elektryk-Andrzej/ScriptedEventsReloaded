using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using MEC;
using SER.Helpers;
using SER.Helpers.Extensions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using UnityEngine;

namespace SER.MethodSystem.Methods.LightMethods;

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
    
    // this took 3 hours because NW doesnt want you setting colors for specific light sources?????
    private static IEnumerator<float> TransitionColor(Room room, Color targetColor, float duration)
    {
        Dictionary<LightsController, Color> startColor = [];
        foreach (var lightsController in room.AllLightControllers)
        {
            var roomLights = lightsController.Base.transform.parent.GetComponentsInChildren<RoomLight>(true);
            var startColorForLight = ColorUtils.AverageColor(
                roomLights
                    .Select(l => l._overrideColorSet ? l._overrideColor : l._initialLightColor)
                    .Where(c => c != Color.clear)
                    .ToArray());

            // Logger.Info(
            //     $"{string.Join(", ", roomLights.Select(l => 
            //         l._overrideColorSet ? l._overrideColor : l._initialLightColor).Where(c => c != Color.clear).ToArray())} " +
            //     $"-> {startColorForLight}");
            
            startColor[lightsController] = startColorForLight;
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            
            foreach (var lightsController in room.AllLightControllers)
            {
                Color currentColor = Color.Lerp(startColor[lightsController], targetColor, t);
                lightsController.OverrideLightsColor = currentColor;
                yield return Timing.WaitForOneFrame;
            }
        }
        
        room.AllLightControllers.ForEachItem(ctrl => ctrl.OverrideLightsColor = targetColor);
    }
}