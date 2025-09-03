using System.Collections.Generic;
using SER.ScriptSystem.ContextSystem.Extensions;
using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.Control;

public class IfStatementContext : TreeContext
{
    private readonly List<BaseToken> _condition = [];
    public ElseStatementContext? ElseStatement { get; set; }

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        _condition.Add(token);
        return TryAddTokenRes.Continue();
    }

    public override Result VerifyCurrentState()
    {
        return _condition is not null
            ? true
            : "An if statement expects to have a condition, but none was provided!";
    }

    protected override IEnumerator<float> Execute()
    {
        if (_condition is null)
        {
            yield break;
        }

        if (ExpressionSystem.EvalCondition(_condition.ToArray(), Script).HasErrored(out var error, out var result))
        {
            Log.Error(Script, $"Error while evaluating condition: {error}");
            yield break;
        }
        
        if (result == false)
        {
            if (ElseStatement is null) yield break;
            
            var enumerator = ElseStatement.Run();
            while (enumerator.MoveNext())
            {
                if (!Script.IsRunning)
                {
                    yield break;
                }
                
                yield return enumerator.Current;
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

    protected override void OnReceivedControlMessageFromChild(ParentContextControlMessage msg)
    {
        ParentContext?.SendControlMessage(msg);
    }
}