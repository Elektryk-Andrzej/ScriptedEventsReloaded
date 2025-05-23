using System;
using System.Collections.Generic;
using LabApi.Features.Enums;
using MapGeneration;

namespace SER.Plugin.HelpSystem;

public static class HelpInfoStorage
{
    public static readonly HashSet<Type> UsedEnums =
    [
        typeof(RoomName),
        typeof(FacilityZone),
        typeof(DoorName)
    ];
}