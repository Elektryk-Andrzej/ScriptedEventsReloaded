using System;
using System.Collections.Generic;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Extensions;
using SER.ContextSystem.Structures;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ContextSystem.Contexts.Control;

public class IfStatementContext : StatementContext, IExtendableStatement, IKeywordContext
{
    public string KeywordName => "if";
    public string Description => "This statement will execute only if the provided condition is met.";
    public string[] Arguments => ["[condition]"];
    
    public IExtendableStatement.Signal AllowedSignals => IExtendableStatement.Signal.DidntExecute;
    public Dictionary<IExtendableStatement.Signal, Func<IEnumerator<float>>> RegisteredSignals { get; } = [];

    private readonly List<BaseToken> _condition = [];

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        _condition.Add(token);
        return TryAddTokenRes.Continue();
    }

    public override Result VerifyCurrentState()
    {
        return _condition.Count > 0
            ? true
            : "An if statement expects to have a condition, but none was provided!";
    }

    protected override IEnumerator<float> Execute()
    {
        if (NumericExpressionReslover.EvalCondition(_condition.ToArray(), Script).HasErrored(out var error, out var result))
        {
            throw new ScriptRuntimeError($"'if' statement condition error: {error}");
        }
        
        if (!result)
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
        
        foreach (var child in Children)
        {
            if (!Script.IsRunning)
            {
                yield break;
            }
            
            var coro = child.ExecuteBaseContext();
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