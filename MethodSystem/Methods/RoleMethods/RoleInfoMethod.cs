using System;
using PlayerRoles;
using SER.Helpers.Exceptions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.RoleMethods;

public class RoleInfoMethod : ReferenceResolvingMethod
{
    public override Type ReferenceType => typeof(PlayerRoleBase);

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new ReferenceArgument<PlayerRoleBase>("playerRole"),
        new OptionsArgument("property",
            Option.Enum<RoleTypeId>("role"),
            Option.Enum<Team>("team"),
            Option.Enum<RoleSpawnFlags>("spawnFlags"), 
            Option.Enum<RoleChangeReason>("spawnReason"),
            "roleName")
    ];

    public override void Execute()
    {
        var role = Args.GetReference<PlayerRoleBase>("playerRole");
        TextReturn = Args.GetOption("property") switch
        {
            "role" => role.RoleTypeId.ToString(),
            "team" => role.Team.ToString(),
            "spawnflags" => role.ServerSpawnFlags.ToString(),
            "spawnreason" => role.ServerSpawnReason.ToString(),
            "rolename" => role.RoleName,
            _ => throw new AndrzejFuckedUpException("out of range")
        };
    }
}