using RemoteAdmin;
using SER.Helpers.Extensions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.ServerMethods;

public class CommandMethod : SynchronousMethod, IAdditionalDescription
{
    public override string Description => "Runs a server command with full permission.";

    public string AdditionalDescription
        => "This action executes commands as the server. Therefore, the command needs '/' before it if it's a RA " +
           "command, or '.' before it if it's a console command.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new TextArgument("command"),
        new PlayerArgument("sender")
        {
            DefaultValue = null
        }
    ];

    public override void Execute()
    {
        var sender = Args.GetSinglePlayer("sender").MaybeNull();
        GameCore.Console.singleton.TypeCommand(
            Args.GetText("command"), 
            sender is not null 
                ? new PlayerCommandSender(sender.ReferenceHub) 
                : null);
    }
}