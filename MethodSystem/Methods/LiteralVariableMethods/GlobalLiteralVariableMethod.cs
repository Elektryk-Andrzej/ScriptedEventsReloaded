using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Exceptions;
using SER.MethodSystem.BaseMethods;
using SER.VariableSystem;
using SER.VariableSystem.Bases;
using SER.VariableSystem.Variables;

namespace SER.MethodSystem.Methods.LiteralVariableMethods;

public class GlobalLiteralVariableMethod : SynchronousMethod
{
    public override string Description => "Creates a global literal variable.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new LiteralVariableArgument("variable to make global")
    ];
    
    public override void Execute()
    {
        var variableToken = Args.GetLiteralVariable("variable to make global");
        if (Script.TryGetVariable<LiteralVariable>(variableToken).HasErrored(out var error, out var variable))
        {
            throw new ScriptRuntimeError(error);
        }

        LiteralVariableIndex.GlobalLiteralVariables
            .RemoveWhere(existingVar => existingVar.Name == variable.Name);
        
        LiteralVariableIndex.GlobalLiteralVariables
            .Add((LiteralVariable)Variable.CopyVariable(variable));
    }
}