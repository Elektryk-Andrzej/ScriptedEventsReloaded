using System;
using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using MapGeneration;
using PlayerRoles;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.VariableSystem;

namespace SER.ScriptSystem.TokenSystem.Tokens.LiteralVariables;

public class PlayerPropertyAccessToken(string initRep = "") : BaseToken(initRep), ILiteralValueSyntaxToken
{
    private string PlayerVarName => RawRepresentation.Split('.')[0];

    private string Property => RawRepresentation.Split('.')[1];

    public static readonly Dictionary<Union<string, string[]>, (string description, Func<Player, string> getAction)>
        AccessiblePlayerProperties = new()
        {
            ["name"] = ("Name", plr => plr.Nickname),
            ["displayName"] = ("Override name", plr => plr.DisplayName),
            ["role"] = ($"Current role ({nameof(RoleTypeId)} enum value)", plr => plr.Role.ToString()),
            ["team"] = ($"Current team ({nameof(Team)} enum value)", plr => plr.Role.GetTeam().ToString()),
            ["serverID"] = ("ID assigned by the server e.g. 2", plr => plr.PlayerId.ToString()),
            ["userID"] = ("ID of the user itself e.g. 1234567890@steam", plr => plr.UserId),
            ["room"] = (
                "A reference to the room the player is currently in, or 'none' if they are not in a room.",
                plr => plr.Room is not null ? ObjectReferenceSystem.RegisterObject(plr.Room) : "none"),
            ["zone"] = ($"Current zone ({nameof(FacilityZone)} enum value)", plr => plr.Zone.ToString()),
            [new[] { "posX", "positionX" }] = ("X position", plr => plr.Position.x.ToString()),
            [new[] { "posY", "positionY" }] = ("Y position", plr => plr.Position.y.ToString()),
            [new[] { "posZ", "positionZ" }] = ("Z position", plr => plr.Position.z.ToString()),
            [new[] { "pos", "position" }] = ("(X, Y, Z) position vector", plr => $"({plr.Position.x}, {plr.Position.y}, {plr.Position.z})"),
            [new[] { "health", "HP" }] = ("Current health", plr => plr.Health.ToString()),
            [new[] { "maxHealth", "maxHP" }] = ("Maximum health", plr => plr.MaxHealth.ToString()),
            [new[] { "AHP" }] = ("Current artificial health", plr => plr.ArtificialHealth.ToString()),
            [new[] { "maxAHP" }] = ("Maximum artificial health", plr => plr.MaxArtificialHealth.ToString()),
            [new[] { "HS" }] = ("Current hume shield", plr => plr.HumeShield.ToString()),
            [new[] { "maxHS" }] = ("Maximum hume shield", plr => plr.MaxHumeShield.ToString()),
            ["IP"] = ("IP adress", plr => plr.IpAddress),
            ["heldItem"] = (
                "A reference to the currently held item, or 'none' if no item is held.",
                plr => plr.CurrentItem is not null ? ObjectReferenceSystem.RegisterObject(plr.CurrentItem) : "none"),
            ["itemAmount"] = ("Amount of items in inventory", plr => plr.Items.Count(i => i is not AmmoItem).ToString()),
        };


    public static readonly Dictionary<string, (string description, Func<Player, string> getAction)>
        CaseInsensitiveAccessiblePlayerProperties =
            AccessiblePlayerProperties
                .SelectMany(kvp =>
                {
                    if (kvp.Key.Item1 is { } str)
                    {
                        return
                        [
                            new KeyValuePair<string, (string description, Func<Player, string> getAction)>(
                                str.ToLower(), kvp.Value)
                        ];
                    }

                    if (kvp.Key.Item2 is { } array)
                    {
                        return array.Select(alias =>
                            new KeyValuePair<string, (string description, Func<Player, string> getAction)>(
                                alias.ToLower(), kvp.Value));
                    }

                    throw new AndrzejFuckedUpException();
                })
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    
    public override bool EndParsingOnChar(char c, out BaseToken? replaceToken)
    {
        replaceToken = null;
        return !char.IsLetter(c);
    }

    public override Result IsValidSyntax()
    {
        var parts = RawRepresentation.Split('.');
        if (parts.Length != 2)
        {
            throw new AndrzejFuckedUpException();
        }
        
        if (!CaseInsensitiveAccessiblePlayerProperties.ContainsKey(parts[1].ToLower()))
        {
            return $"Value '{parts[1]}' is not a valid player property.";
        }

        return true;
    }

    public TryGet<string> TryGetValue(Script scr)
    {
        var rs = new ResultStacker($"Failed to get value of player property access '{RawRepresentation}'");

        if (scr.TryGetPlayerVariable(PlayerVarName).HasErrored(out var varErr, out var playerVariable))
        {
            return rs.Add(varErr);
        }

        if (playerVariable.Players.Len() != 1)
        {
            return rs.Add(
                $"Player variable '{PlayerVarName}' has to contain exactly 1 player, " +
                $"but has {playerVariable.Players.Len()} instead.");
        }

        if (!CaseInsensitiveAccessiblePlayerProperties.TryGetValue(Property.ToLower(), out var info))
        {
            return rs.Add($"Value '{Property}' is not a valid player property.");
        }
        
        return TryGet<string>.Success(info.getAction(playerVariable.Players.First()));
    }
}