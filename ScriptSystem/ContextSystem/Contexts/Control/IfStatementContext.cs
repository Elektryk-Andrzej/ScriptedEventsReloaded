using System;
using System.Collections.Generic;
using SER.ScriptSystem.ContextSystem.Extensions;
using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.Control;

public class IfStatementContext : StatementContext, IExtendableStatement, IKeywordContext
{
    public string Keyword => "if";

    public string Description =>
        "This statement will execute only if the provided condition is met.";

    public string Arguments => "condition";
    
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

    public override IEnumerator<float> Execute()
    {
        if (ExpressionSystem.EvalCondition(_condition.ToArray(), Script).HasErrored(out var error, out var result))
        {
            Log.Error(Script, $"'if' statement condition error: {error}");
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