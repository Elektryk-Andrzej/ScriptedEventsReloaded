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

public class RoomArgument(string name) : EnumHandlingArgument(name)
{
    public override string InputDescription => $"{nameof(RoomName)} enum or reference to {nameof(Room)}";

    [UsedImplicitly]
    public DynamicTryGet<Room> GetConvertSolution(BaseToken token)
    {
        return ResolveEnums<Room>(
            token,
            new()
            {
                [typeof(RoomName)] = roomName => Room.List.First(room => room.Name == (RoomName)roomName),
            },
            () =>
            {
                Result rs = $"Value '{token.RawRep}' cannot be interpreted as {InputDescription}.";
                if (token is not IValueCapableToken<ReferenceValue> refToken)
                {
                    return rs;
                }

                return new(() =>
                {
                    if (ReferenceArgument<Room>.TryParse(refToken).WasSuccessful(out var room))
                    {
                        return room;
                    }

                    return rs;
                });
            }
        );
    }
}