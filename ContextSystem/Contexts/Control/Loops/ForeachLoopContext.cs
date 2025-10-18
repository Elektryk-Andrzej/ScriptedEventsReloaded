using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Extensions;
using SER.ContextSystem.Structures;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ContextSystem.Contexts.Control.Loops;

[UsedImplicitly]
public class ForeachLoopContext : LoopContext
{
    private readonly Result _mainErr = "Cannot create 'foreach' loop.";
    
    private PlayerVariableToken? _iterationVariableToken;
    private bool _usedInKeyword = false;
    private PlayerVariableToken? _iterableToken;

    public override string KeywordName => "foreach";
    public override string Description =>
        "Repeats its body for each player in the player variable, assigning it its own custom variable.";
    public override string[] Arguments => ["[player variable]", "in", "[player variable]"];

    public override Dictionary<IExtendableStatement.Signal, Func<IEnumerator<float>>> RegisteredSignals { get; } =
        new();

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        if (_iterationVariableToken is null)
        {
            if (token is not PlayerVariableToken varToken)
            {
                return TryAddTokenRes.Error("Foreach loop expects to have a player variable as its first argument.");
            }
            
            _iterationVariableToken = varToken;
            return TryAddTokenRes.Continue();
        }

        if (!_usedInKeyword)
        {
            if (token.RawRepresentation != "in")
            {
                return TryAddTokenRes.Error("Foreach loop expects to have 'in' keyword as its second argument.");
            }
            
            _usedInKeyword = true;
            return TryAddTokenRes.Continue();
        }

        if (token is not PlayerVariableToken iterable)
        {
            return TryAddTokenRes.Error("Foreach loop expects to have a player variable as its third argument.");
        }
        
        _iterableToken = iterable;
        return TryAddTokenRes.End();
    }

    public override Result VerifyCurrentState()
    {
        return Result.Assert(
            _iterationVariableToken is not null && _iterableToken is not null && _usedInKeyword,
            _mainErr + "Missing required arguments.");
    }

    protected override IEnumerator<float> Execute()
    {
        Result rs = "foreach loop cannot execute";
        
        if (Script.TryGetPlayerVariable(_iterationVariableToken!).WasSuccessful())
        {
            throw new ScriptErrorException(rs + $"Variable {_iterationVariableToken!.Name} already exists.");
        }
        
        if (Script.TryGetPlayerVariable(_iterableToken!).HasErrored(out var error, out var iterable))
        {
            throw new ScriptErrorException(error);
        }

        foreach (var plr in new List<Player>(iterable.Players))
        {
            Script.AddLocalPlayerVariable(new(_iterationVariableToken!.Name, [plr]));
            
            foreach (var coro in Children.Select(child => child.ExecuteBaseContext()))
            {
                while (coro.MoveNext())
                {
                    yield return coro.Current;
                }
                
                if (ExitLoop)
                {
                    break;
                }

                if (SkipThisIteration)
                {
                    SkipThisIteration = false;
                    break;
                }
            }
            
            Script.RemoveLocalPlayerVariable(_iterationVariableToken!.Name);
            
            if (ExitLoop)
            {
                break;
            }
        }
    }
}