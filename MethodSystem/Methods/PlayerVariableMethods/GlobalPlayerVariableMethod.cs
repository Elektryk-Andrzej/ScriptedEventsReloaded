using System;
using System.Linq;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.VariableSystem;
using SER.VariableSystem.Variables;

namespace SER.MethodSystem.Methods.PlayerVariableMethods;

public class GlobalPlayerVariableMethod : SynchronousMethod
{
    public override string Description => "Creates or overrides a global player variable.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayerVariableNameArgument("variableName")
        {
            Description = "Warning! DO NOT use a variable name that is already predefined by SER"
        },
        new PlayersArgument("players"),
    ];
    
    public override void Execute()
    {
        var variableName = Args.GetPlayerVariableName("variableName");
        var players = Args.GetPlayers("players");

        if (PlayerVariableIndex.GlobalPlayerVariables.OfType<PredefinedPlayerVariable>()
            .Any(var => var.Name == variableName.Name))
        {
            throw new Exception($"You cannot override the predefined variable @{variableName.Name}");
        }
        
        PlayerVariableIndex.GlobalPlayerVariables.RemoveWhere(var => 
            var.Name == variableName.Name);

        PlayerVariableIndex.GlobalPlayerVariables.Add(new(variableName.Name, new(players)));
    }
}