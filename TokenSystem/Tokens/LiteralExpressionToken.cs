using System;
using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using PlayerRoles;
using SER.ContextSystem;
using SER.ContextSystem.Contexts;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem;
using SER.ScriptSystem.Structures;
using SER.TokenSystem.Slices;
using SER.TokenSystem.Structures;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class LiteralExpressionToken : BaseToken, ILiteralValueToken
{
    public interface IExpressionType
    {
        public TryGet<LiteralValue> Handler();
    }

    public IExpressionType Type { get; private set; } = null!;

    public TryGet<LiteralValue> GetLiteralValue(Script scr)
    {
        return Type.Handler();
    }

    public static TryGet<LiteralExpressionToken> TryGet(string entireExpression, Script script)
    {
        Result mainError = $"Value '{entireExpression}' is not a valid literal expression.";
        if (entireExpression.Length <= 1)
        {
            return mainError + "Expression is too short.";
        }
        
        var firstChar = entireExpression.First();
        if (!CollectionSlice.CollectionSliceInfo.ContainsKey(firstChar))
        {
            return mainError + $"Expression cannot start with '{firstChar}'.";
        }

        var slice = new CollectionSlice(firstChar);
        var requestedEnd = false;
        foreach (var character in entireExpression.Skip(1))
        {
            if (requestedEnd)
            {
                return mainError + $"Expression cannot accept character '{character}'.";
            }
            
            if (!slice.CanContinueAfterAdd(character))
            {
                requestedEnd = true;
            }
        }

        if (slice.VerifyState().HasErrored(out var error))
        {
            return mainError + error;
        }
        
        return TryGet(slice, script);
    }

    public static TryGet<LiteralExpressionToken> TryGet(CollectionSlice slice, Script script)
    {
        var token = new LiteralExpressionToken();
        if (token.TryInit(slice, script, null).HasErrored(out var error))
        {
            return error;
        }

        return token;
    }

    protected override Result InternalParse(Script scr)
    {
        Result error = $"Expression failed while parsing from '{Slice.RawRepresentation}'";
        if (Slice is not CollectionSlice { SliceType: CollectionSliceType.Curly })
        {
            return error + "Slice is not in curly braces.";
        }
        
        if (Tokenizer.TokenizeLine(Slice.Value, scr, null)
            .HasErrored(out var tokenizeError, out var outTkns))
        {
            return tokenizeError;
        }
        
        var tokens = outTkns.ToArray();
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (tokens.Length == 1 && tokens.First() is LiteralVariableToken varToken)
        {
            Type = new VariableExpression { Token = varToken };
            return true;
        }

        if (tokens.Length >= 1 && tokens.First() is MethodToken { Method: ReturningMethod } methodToken)
        {
            if (TryAttachMethod(tokens, scr).HasErrored(out var methodError, out var method))
            {
                return error 
                       + $"(Expression is assumed to be a method expression judging by the '{methodToken.Method.Name}' method)" 
                       + methodError;
            }

            Type = new MethodExpression
            {
                Token = methodToken,
                Method = method
            };
            
            return true;
        }

        if (tokens.Length == 2 && tokens.First() is PlayerVariableToken pvarToken)
        {
            Result pvarError = $"(Expression is assumed to be a player property access expression judging by the '{pvarToken.Name}' player variable)";
            
            var propName = tokens.Last().RawRepresentation;
            if (!Enum.TryParse(propName, true, out PlayerPropertyExpression.PlayerPropertyType property))
            {
                return error
                       + pvarError
                       + $"Value '{propName}' is not a valid {nameof(PlayerPropertyExpression.PlayerPropertyType)} enum value.";
            }

            Type = new PlayerPropertyExpression
            {
                Property = property,
                Token = pvarToken
            };
            
            return true;
        }
        
        return "Expression is not a valid literal variable, method or player property access.";
    }
    
    private static TryGet<ReturningMethod> TryAttachMethod(BaseToken[] tokens, Script scr)
    {
        if (Contexter.ContextLine(tokens, null, scr)
            .HasErrored(out var contextError, out var context))
        {
            return contextError;
        }

        if (context is not MethodContext methodContext)
        {
            return "Expression is not a method that can be ran.";
        }

        if (methodContext.Method is not ReturningMethod method)
        {
            return "Expression is a method, but this method doesn't return any value.";
        }
        
        return method;
    }
}

public class VariableExpression : LiteralExpressionToken.IExpressionType
{
    public required LiteralVariableToken Token { get; init; }
    
    public TryGet<LiteralValue> Handler()
    {
        if (Token.TryGetVariable().HasErrored(out var err, out var variable))
        {
            return err;
        }
        
        return variable.BaseValue;
    }
}
    
public class MethodExpression : LiteralExpressionToken.IExpressionType
{
    public required MethodToken Token { get; init; }
    public required ReturningMethod Method { get; init; }
    
    public TryGet<LiteralValue> Handler()
    {
        Method.Execute();
        if (Method.Value is null)
        {
            return "Method did not return any value.";
        }
        
        return Method.Value;
    }
}

public class PlayerPropertyExpression : LiteralExpressionToken.IExpressionType
{
    public required PlayerVariableToken Token { get; init; }
    public required PlayerPropertyType Property { get; init; }
    
    public enum PlayerPropertyType
    {
        Nickname,
        DisplayName,
        Role,
        RoleReference,
        Team,
        IsAlive,
        UserId,
        PlayerId,
        CustomInfo,
        Health,
        MaxHealth,
        ArtificialHealth,
        MaxArtificialHealth,
        HumeShield,
        MaxHumeShield,
        HumeShieldRegenRate,
        GroupName,
        IsDisarmed,
        IsMuted,
        IsIntercomMuted,
        IsGlobalModerator,
        IsNorthwoodStaff,
        IsBypassEnabled,
        IsGodModeEnabled,
        IsNoclipEnabled,
    }

    public abstract class Info
    {
        public abstract Func<Player, LiteralValue> Handler { get; }
        public abstract Type ReturnType { get; }
        public abstract string? Description { get; }
    }

    public class Info<T>(Func<Player, T> handler, string? description) : Info 
        where T : LiteralValue
    {
        public override Func<Player, LiteralValue> Handler => handler;
        public override Type ReturnType => typeof(T);
        public override string? Description => description;
    }

    public static readonly Dictionary<PlayerPropertyType, Info> PropertyInfoMap = new()
    {
        [PlayerPropertyType.Nickname] = new Info<TextValue>(plr => plr.Nickname, null),
        [PlayerPropertyType.DisplayName] = new Info<TextValue>(plr => plr.DisplayName, null),
        [PlayerPropertyType.Role] = new Info<TextValue>(plr => plr.Role.ToString(), $"Player role type ({nameof(RoleTypeId)} enum value)"),
        [PlayerPropertyType.RoleReference] = new Info<ReferenceValue>(plr => new ReferenceValue(plr.RoleBase), $"Reference to {nameof(PlayerRoleBase)}"),
        [PlayerPropertyType.Team] = new Info<TextValue>(plr => plr.Team.ToString(), $"Player team ({nameof(Team)} enum value)"),
        [PlayerPropertyType.IsAlive] = new Info<BoolValue>(plr => plr.IsAlive, null),
        [PlayerPropertyType.UserId] = new Info<TextValue>(plr => plr.UserId, "The ID of the account (like SteamID64)"),
        [PlayerPropertyType.PlayerId] = new Info<NumberValue>(plr => plr.PlayerId, "The ID that the server assigned for this round"),
        [PlayerPropertyType.CustomInfo] = new Info<TextValue>(plr => plr.CustomInfo, "Custom info set by the server"),
        [PlayerPropertyType.Health] = new Info<NumberValue>(plr => (decimal)plr.Health, null),
        [PlayerPropertyType.MaxHealth] = new Info<NumberValue>(plr => (decimal)plr.MaxHealth, null),
        [PlayerPropertyType.ArtificialHealth] = new Info<NumberValue>(plr => (decimal)plr.ArtificialHealth, null),
        [PlayerPropertyType.MaxArtificialHealth] = new Info<NumberValue>(plr => (decimal)plr.MaxArtificialHealth, null),
        [PlayerPropertyType.HumeShield] = new Info<NumberValue>(plr => (decimal)plr.HumeShield, null),
        [PlayerPropertyType.MaxHumeShield] = new Info<NumberValue>(plr => (decimal)plr.MaxHumeShield, null),
        [PlayerPropertyType.HumeShieldRegenRate] = new Info<NumberValue>(plr => (decimal)plr.HumeShieldRegenRate, null),
        [PlayerPropertyType.GroupName] = new Info<TextValue>(plr => plr.GroupName, "The name of the group (like admin or vip)"),
        [PlayerPropertyType.IsDisarmed] = new Info<BoolValue>(plr => plr.IsDisarmed, null),
        [PlayerPropertyType.IsMuted] = new Info<BoolValue>(plr => plr.IsMuted, null),
        [PlayerPropertyType.IsIntercomMuted] = new Info<BoolValue>(plr => plr.IsIntercomMuted, null),
        [PlayerPropertyType.IsGlobalModerator] = new Info<BoolValue>(plr => plr.IsGlobalModerator, null),
        [PlayerPropertyType.IsNorthwoodStaff] = new Info<BoolValue>(plr => plr.IsNorthwoodStaff, null),
        [PlayerPropertyType.IsBypassEnabled] = new Info<BoolValue>(plr => plr.IsBypassEnabled, null),
        [PlayerPropertyType.IsGodModeEnabled] = new Info<BoolValue>(plr => plr.IsGodModeEnabled, null),
        [PlayerPropertyType.IsNoclipEnabled] = new Info<BoolValue>(plr => plr.IsNoclipEnabled, null),
    };

    public TryGet<LiteralValue> Handler()
    {
        if (Token.TryGetVariable().HasErrored(out var err, out var variable))
        {
            return err;
        }

        return variable.Players.Len switch
        {
            < 1 => $"Player variable '{variable.Name}' has no players.",
            > 1 => $"Player variable '{variable.Name}' has more than one player.",
            _ => PropertyInfoMap[Property].Handler(variable.Players.First())
        };
    }
}