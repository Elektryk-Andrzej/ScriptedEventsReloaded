using System;

namespace SER.MethodSystem.ArgumentSystem.Structures;

[Flags]
public enum OperatingValue
{
    DoorName             = 1 << 0,
    RoomName             = 1 << 1,
    FacilityZone         = 1 << 2,
    DoorReferences       = 1 << 3,
    SingleDoorReference  = 1 << 4,
    RoomReferences       = 1 << 5,
    SingleRoomReference  = 1 << 6,
    AllOfType            = 1 << 7,
    Boolean              = 1 << 8,
    Color                = 1 << 9,
    Text                 = 1 << 10,
    Duration             = 1 << 11,
    CustomEnum           = 1 << 12,
    Float                = 1 << 13,
    Int                  = 1 << 14,
    CustomOption         = 1 << 15,
    Players              = 1 << 16,
    Player               = 1 << 17,
    Percentage           = 1 << 18,
    CustomReference      = 1 << 19,
    Script               = 1 << 20,
    Variable             = 1 << 21,
    PlayerVariableName   = 1 << 22,
    ItemType             = 1 << 23,
    ItemReference        = 1 << 24,
    ItemReferences       = 1 << 25,
    LiteralVariableName  = 1 << 26,
    ElevatorGroup        = 1 << 27,
}