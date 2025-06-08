using System;
using System.Collections.Generic;
using Interactables.Interobjects;
using LabApi.Features.Enums;
using MapGeneration;

namespace SER.Plugin.HelpSystem;

public static class HelpInfoStorage
{
    public static readonly HashSet<Type> UsedEnums =
    [
        typeof(RoomName),
        typeof(FacilityZone),
        typeof(DoorName),
        typeof(ItemType),
        typeof(ElevatorGroup)
    ];
}