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

public class ElifStatementContext : TreeContext, ITreeExtender, IExtendableTree
{
    public IExtendableTree.ControlMessage Extends => IExtendableTree.ControlMessage.DidntExecute;
    public IExtendableTree.ControlMessage AllowedControlMessages => IExtendableTree.ControlMessage.DidntExecute;
    public Dictionary<IExtendableTree.ControlMessage, Func<IEnumerator<float>>> ControlMessages { get; } = new();

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
            if (!ControlMessages.TryGetValue(IExtendableTree.ControlMessage.DidntExecute, out var enumerator))
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