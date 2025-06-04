using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;
using SER.ScriptSystem.TokenSystem.Structures;

namespace SER.MethodSystem.Methods.EventMethods;

public class IsAllowedMethod : Method, IAdditionalDescription
{
    public override string Description => "Sets whether or not the event is allowed to run.";

    public string AdditionalDescription =>
        "In order for it to have any impact, the script in which the method is used must be trigger by an event, and " +
        "that event must be cancellable.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new BoolArgument("isAllowed")
    ];

    public override void Execute()
    {
        if (Args.GetBool("isAllowed") == false)
        {
            Script.SendControlMessage(ScriptControlMessage.EventNotAllowed);
        }
    }
}