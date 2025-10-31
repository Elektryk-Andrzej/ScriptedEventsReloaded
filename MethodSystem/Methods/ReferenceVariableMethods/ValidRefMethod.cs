using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.ReferenceVariableMethods;

public class ValidRefMethod : ReturningMethod<BoolValue>
{
    public override string Description => "Verifies if the reference is valid, by checking if the object exists.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new IsValidReferenceArgument("reference")
    ];
    
    public override void Execute()
    {
        ReturnValue = Args.GetIsValidReference("reference");
    }
}