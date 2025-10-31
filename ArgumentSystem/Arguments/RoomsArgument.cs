using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.ValueSystem;

namespace SER.ArgumentSystem.Arguments;

public class RoomsArgument(string name) : EnumHandlingArgument(name)
{
    public override string InputDescription => 
        $"{nameof(RoomName)} enum, {nameof(FacilityZone)} enum, reference to {nameof(Room)}, or * for every room";

    [UsedImplicitly]
    public DynamicTryGet<Room[]> GetConvertSolution(BaseToken token)
    {
        return ResolveEnums<Room[]>(
            token,
            new()
            {
                [typeof(RoomName)] = roomName => Room.List.Where(room => room.Name == (RoomName)roomName).ToArray(),
                [typeof(FacilityZone)] = zone => Room.List.Where(room => room.Zone == (FacilityZone)zone).ToArray(),
            },
            () =>
            {
                Result rs = $"Value '{token.RawRep}' cannot be interpreted as {InputDescription}.";
                if (token is SymbolToken { IsJoker: true })
                {
                    return Room.List.ToArray();
                }

                if (!token.CanReturn<ReferenceValue>(out var get))
                {
                    return rs;
                }

                return new(() =>
                {
                    if (get().HasErrored(out var error, out var refVal))
                    {
                        return error;
                    }
                    
                    if (ReferenceArgument<Room>.TryParse(refVal).WasSuccessful(out var room))
                    {
                        return new[] { room };
                    }

                    return rs;
                });
            }
        );
    }
}