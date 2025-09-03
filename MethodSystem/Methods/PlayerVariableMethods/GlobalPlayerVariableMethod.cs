using System;
using System.Linq;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.VariableSystem;
using SER.VariableSystem.Structures;

namespace SER.MethodSystem.Methods.PlayerVariableMethods;

public class GlobalPlayerVariableMethod : SynchronousMethod
{
    public override string Description => "Creates or overrides a global player variable.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayerVariableNameArgument("variableName")
        {
            Description = "Warning! DO NOT use a variable name that is already predefined by SER, like " +
                          "@alivePlayers, @scpPlayers etc."
        },
        new PlayersArgument("players"),
    ];
    
    public override void Execute()
    {
        var variableName = Args.GetPlayerVariableName("variableName");
        var players = Args.GetPlayers("players");

        if (PlayerVariableIndex.GlobalPlayerVariables.OfType<PredefinedPlayerVariable>()
            .Any(var => var.Name == variableName.NameWithoutPrefix))
        {
            throw new Exception($"You cannot override the predefined variable @{variableName.NameWithoutPrefix}");
        }
        
        PlayerVariableIndex.GlobalPlayerVariables.RemoveWhere(var => 
            var.Name == variableName.NameWithoutPrefix);

        PlayerVariableIndex.GlobalPlayerVariables.Add(new(variableName.NameWithoutPrefix, players));
    }
}