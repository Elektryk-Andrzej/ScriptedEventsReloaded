using System;
using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using MapGeneration;
using PlayerRoles;
using SER.Helpers.Extensions;
using SER.VariableSystem.Variables;

namespace SER.VariableSystem;

public static class PlayerVariableIndex
{
    public static readonly HashSet<PlayerVariable> GlobalPlayerVariables = [];

    public static void Initialize()
    {
        GlobalPlayerVariables.Clear();
        
        List<PredefinedPlayerVariable> allApiVariables =
        [
            new("all", Player.ReadyList.ToList, "Other"),
            new("alivePlayers", () => Player.ReadyList.Where(plr => plr.IsAlive).ToList(), "Other"),
            new("npcPlayers", () => Player.ReadyList.Where(plr => plr.IsNpc).ToList(), "Other"),
            new("empty", () => [], "Other")
        ];

        allApiVariables.AddRange(
            Enum.GetValues(typeof(RoleTypeId))
                .Cast<RoleTypeId>()
                .Where(role => role is not RoleTypeId.None)
                .Select(roleType =>
                {
                    return new PredefinedPlayerVariable(roleType.ToString().LowerFirst() + "Players",
                        () => Player.ReadyList.Where(plr => plr.Role == roleType).ToList(),
                        "Role");
                }));
        
        allApiVariables.AddRange(
            Enum.GetValues(typeof(FacilityZone))
                .Cast<FacilityZone>()
                .Where(zone => zone is not FacilityZone.None and not FacilityZone.Other)
                .Select(zone =>
                {
                    return new PredefinedPlayerVariable(zone.ToString().LowerFirst() + "Players",
                        () => Player.ReadyList.Where(plr => plr.Zone == zone).ToList(),
                        "Facility zone");
                }));
        
        allApiVariables.AddRange(
            Enum.GetValues(typeof(Team))
                .Cast<Team>()
                .Select(teamType =>
                {
                    string name = teamType.ToString();
                    if (teamType is Team.SCPs)
                    {
                        name = "scp";
                    }
                    else if (name.EndsWith("s"))
                    {
                        name = name.Substring(0, name.Length - 1);
                    }

                    name = name.LowerFirst() + "Players";
                    if (allApiVariables.Any(v => v.Name == name))
                    {
                        return null;
                    }

                    return new PredefinedPlayerVariable(name,
                        () => Player.ReadyList.Where(plr => plr.Role.GetTeam() == teamType).ToList(),
                        "Team");
                })
                .OfType<PredefinedPlayerVariable>());

        foreach (var variable in allApiVariables)
        {
            GlobalPlayerVariables.Add(variable);
        }
    }
}