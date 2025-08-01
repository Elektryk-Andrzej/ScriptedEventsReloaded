﻿using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.VariableSystem;

namespace SER.MethodSystem.Methods.LiteralVariableMethods;

public class GlobalLiteralVariableMethod : Method
{
    public override string Description => "Creates or overrides a global literal variable.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new LiteralVariableNameArgument("variableName"),
        new TextArgument("value")
    ];
    
    public override void Execute()
    {
        var variableName = Args.GetLiteralVariableName("variableName").ValueWithoutBrackets;
        var value = Args.GetText("value");

        LiteralVariableIndex.GlobalLiteralVariables
            .RemoveWhere(existingVar => existingVar.Name == variableName);
        
        LiteralVariableIndex.GlobalLiteralVariables.Add(new()
        {
            Name = variableName,
            Value = () => value
        });
    }
}