using System;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.VariableSystem;

namespace SER.MethodSystem.Methods.GeneralVariableMethods;

public class PopVariableMethod : ReturningMethod
{
    public override string? Description => "Erases a given variable, returning its value.";

    public override Type[]? ReturnTypes => null;

    public override Argument[] ExpectedArguments { get; } =
    [  
        new OptionsArgument("target", "local", "global"),
        new VariableArgument("variable to remove")
        {
            Description = "This only works on local variables!"
        }
    ];

    public override void Execute()
    {
        var variable = Args.GetVariable("variable to remove");
        switch (Args.GetOption("target"))
        {
            case "local": 
                Script.RemoveVariable(variable);
                break;
            case "global":
                VariableIndex.GlobalVariables.RemoveAll(existingVar => existingVar.Name == variable.Name);
                break;
        }
        
        ReturnValue = variable.BaseValue;
    }
}