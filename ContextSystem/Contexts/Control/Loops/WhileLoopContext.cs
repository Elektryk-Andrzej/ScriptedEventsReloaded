using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Extensions;
using SER.ContextSystem.Structures;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ContextSystem.Contexts.Control.Loops;

[UsedImplicitly]
public class WhileLoopContext : StatementContext, IKeywordContext, IExtendableStatement
{
    private readonly Result _rs = "Cannot create 'while' loop.";
    private readonly List<BaseToken> _condition = []; 
    private bool _skipChild = false;
    
    public string KeywordName => "while";
    public string Description =>
        "A statement which will execute its body as long as the provided condition is evaluated to true.";
    public string[] Arguments => ["[condition...]"];

    public IExtendableStatement.Signal AllowedSignals =>
        IExtendableStatement.Signal.DidntExecute | IExtendableStatement.Signal.EndedExecution;
    public Dictionary<IExtendableStatement.Signal, Func<IEnumerator<float>>> RegisteredSignals { get; } = [];

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        _condition.Add(token);
        return TryAddTokenRes.Continue();
    }

    public override Result VerifyCurrentState()
    {
        return Result.Assert(
            _condition.Count > 0,
            _rs + "The condition was not provided.");
    }

    protected override IEnumerator<float> Execute()
    {
        if (NumericExpressionReslover.EvalCondition(_condition.ToArray(), Script).HasErrored(out var error, out var condition))
        {
            throw new ScriptRuntimeError(error);
        }
        
        while (condition)
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var child in Children)
            {
                var coro = child.ExecuteBaseContext();
                while (coro.MoveNext())
                {
                    yield return coro.Current;
                }

                if (!_skipChild) continue;

                _skipChild = false;
                break;
            }
            
            if (NumericExpressionReslover.EvalCondition(_condition.ToArray(), Script).HasErrored(out var error2, out condition))
            {
                throw new ScriptRuntimeError(error2);
            }
        }

        if (RegisteredSignals.TryGetValue(IExtendableStatement.Signal.EndedExecution, out var coroFunc))
        {
            var coro = coroFunc();
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
        if (msg == ParentContextControlMessage.LoopContinue)
        {
            _skipChild = true;
            return;
        }

        ParentContext?.SendControlMessage(msg);
    }
}