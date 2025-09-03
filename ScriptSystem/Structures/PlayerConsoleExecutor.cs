using RemoteAdmin;
using UnityEngine;

namespace SER.ScriptSystem.Structures;

public class PlayerConsoleExecutor : ScriptExecutor
{
    public required ReferenceHub Sender { get; init; }

    public override void Reply(string content, Script scr)
    {
        Sender.gameConsoleTransmission.SendToClient(content, "green");
    }

    public override void Warn(string content, Script scr)
    {
        Sender.gameConsoleTransmission.SendToClient(content, "yellow");
    }

    public override void Error(string content, Script scr)
    {
        Sender.gameConsoleTransmission.SendToClient(content, "red");
    }
}