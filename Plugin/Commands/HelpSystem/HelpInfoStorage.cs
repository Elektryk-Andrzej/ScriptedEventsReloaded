using System;
using System.Collections.Generic;
using Interactables.Interobjects;
using LabApi.Features.Enums;
using MapGeneration;
using SER.FlagSystem.Flags;
using SER.TokenSystem.Tokens.ExpressionTokens;

namespace SER.Plugin.Commands.HelpSystem;

public static class HelpInfoStorage
{
    public static readonly HashSet<Type> UsedEnums =
    [
        typeof(RoomName),
        typeof(FacilityZone),
        typeof(DoorName),
        typeof(ItemType),
        typeof(ElevatorGroup),
        typeof(CustomCommandFlag.ConsoleType),
        typeof(PlayerExpressionToken.PlayerProperty)
    ];
}