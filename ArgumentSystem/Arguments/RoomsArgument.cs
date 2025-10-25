using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
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

                if (token is not IValueCapableToken<ReferenceValue> refToken)
                {
                    return rs;
                }

                return new(() =>
                {
                    if (ReferenceArgument<Room>.TryParse(refToken).WasSuccessful(out var room))
                    {
                        return new[] { room };
                    }

                    return rs;
                });
            }
        );
    }
}