using System;

namespace SER.ScriptSystem.FlagSystem.Structures;

[Flags]
public enum ConsoleType
{
    None        = 0,
    Player      = 1 << 0,
    RemoteAdmin = 1 << 1,
    Server      = 1 << 2
}