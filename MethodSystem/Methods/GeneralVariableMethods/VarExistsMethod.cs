using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.TokenSystem.Tokens.Variables;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.GeneralVariableMethods;

public class VarExistsMethod : ReturningMethod<BoolValue>
{
    public override string Description => "Returns a bool value indicating if the provided variable exists.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new TokenArgument<VariableToken>("variable")
    ];
    
    public override void Execute()
    {
        var token = Args.GetToken<VariableToken>("variable");
        ReturnValue = token.TryGetVariable().WasSuccessful();
    }
}