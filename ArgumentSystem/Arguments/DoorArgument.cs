using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ArgumentSystem.Arguments;

public class DoorArgument(string name) : EnumHandlingArgument(name)
{
    public override string InputDescription => $"{nameof(DoorName)} enum, {nameof(FacilityZone)} enum or reference to {nameof(Door)}";

    [UsedImplicitly]
    public DynamicTryGet<Door> GetConvertSolution(BaseToken token)
    {
        return ResolveEnums<Door>(
            token,
            new()
            {
                [typeof(DoorName)] = doorName =>
                {
                    var door = Door.List.Where(door => door.DoorName == (DoorName)doorName).GetRandomValue();
                    if (door is null)
                    {
                        return $"Door with name '{doorName}' does not exist.";
                    }

                    return door;
                },
                [typeof(FacilityZone)] = zone =>
                {
                    var door = Door.List.Where(d => d.Zone == (FacilityZone)zone)
                        .Where(d => d is not ElevatorDoor)
                        .GetRandomValue();
                    if (door is null)
                    {
                        return $"No doors in zone '{zone}' exist.";
                    }

                    return door;
                }
            },
            () =>
            {
                if (ReferenceArgument<Door>.TryParse(token, Script).WasSuccessful(out var door))
                {
                    return door;
                }

                return $"Value '{token.RawRepresentation}' cannot be interpreted as a door.";
            }
        );
    }
}