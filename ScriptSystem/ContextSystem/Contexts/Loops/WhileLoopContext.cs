using System.Collections.Generic;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.ScriptSystem.ContextSystem.Extensions;
using SER.ScriptSystem.TokenSystem;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.Loops;

public class WhileLoopContext : TreeContext
{
    private readonly ResultStacker _rs = new("Cannot create `while` loop.");
    private string? _condition = null;
    private bool _skipChild = false;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        _condition = token.GetValue();
        return TryAddTokenRes.End();
    }

    public override Result VerifyCurrentState()
    {
        return Result.Assert(
            _condition is not null,
            _rs.Add("The condition was not provided."));
    }

    protected override IEnumerator<float> Execute()
    {
        if (ExpressionSystem.EvalCondition(_condition!, Script).HasErrored(out var error, out var condition))
        {
            throw new MalformedConditionException(error);
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
            
            if (ExpressionSystem.EvalCondition(_condition!, Script).HasErrored(out var error2, out condition))
            {
                throw new MalformedConditionException(error2);
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