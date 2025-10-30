using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
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
                if (token is not IValueToken valToken || valToken.CanReturn<ReferenceValue>(out var get))
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
                        return room;
                    }

                    return rs;
                });
            }
        );
    }
}