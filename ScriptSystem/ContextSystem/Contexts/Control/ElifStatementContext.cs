using System;
using System.Collections.Generic;
using System.Linq;
using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Extensions;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.Control;

public class ElifStatementContext : StatementContext, IStatementExtender, IExtendableStatement, IKeywordContext
{
    public string Keyword => "elif";
    public string Description =>
        "If the statement above it didn't execute, 'elif' statement will try to execute if the provided condition is met.";
    public string Arguments => "condition";

    public IExtendableStatement.Signal Extends => IExtendableStatement.Signal.DidntExecute;
    
    public IExtendableStatement.Signal AllowedSignals => IExtendableStatement.Signal.DidntExecute;
    public Dictionary<IExtendableStatement.Signal, Func<IEnumerator<float>>> RegisteredSignals { get; } = new();

    private readonly List<BaseToken> _condition = [];
    
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        _condition.Add(token);
        return TryAddTokenRes.Continue();
    }

    public override Result VerifyCurrentState()
    {
        return Result.Assert(
            _condition.Count > 0,
            "An elif statement expects to have a condition, but none was provided!");
    }

    public override IEnumerator<float> Execute()
    {
        if (ExpressionSystem.EvalCondition(_condition.ToArray(), Script).HasErrored(out var error, out var result))
        {
            Log.Error(Script, $"'elif' statement condition error: {error}");
            yield break;
        }
        
        if (result == false)
        {
            if (!RegisteredSignals.TryGetValue(IExtendableStatement.Signal.DidntExecute, out var enumerator))
            {
                yield break;
            }
            
            var coro = enumerator();
            while (coro.MoveNext())
            {
                if (!Script.IsRunning)
                {
                    yield break;
                }
                
                yield return coro.Current;
            }
            
            yield break;
        }
        
        foreach (var coro in Children
                     .TakeWhile(_ => Script.IsRunning)
                     .Select(child => child.ExecuteBaseContext()))
        {
            while (coro.MoveNext())
            {
                if (!Script.IsRunning)
                {
                    yield break;
                }
                
                yield return coro.Current;
            }
        }
    }
}