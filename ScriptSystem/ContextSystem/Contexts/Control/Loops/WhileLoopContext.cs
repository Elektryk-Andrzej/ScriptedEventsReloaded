using System.Collections.Generic;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Extensions;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.Control.Loops;

public class WhileLoopContext : StatementContext, IKeywordContext
{
    private readonly ResultStacker _rs = new("Cannot create `while` loop.");
    private readonly List<BaseToken> _condition = []; 
    private bool _skipChild = false;
    
    public string Keyword => "while";

    public string Description =>
        "A statement which will execute its body as long as the provided condition is evaluated to true.";

    public string Arguments => "condition";

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        _condition.Add(token);
        return TryAddTokenRes.Continue();
    }

    public override Result VerifyCurrentState()
    {
        return Result.Assert(
            _condition.Count > 0,
            _rs.Add("The condition was not provided."));
    }

    public override IEnumerator<float> Execute()
    {
        if (ExpressionSystem.EvalCondition(_condition.ToArray(), Script).HasErrored(out var error, out var condition))
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
            
            if (ExpressionSystem.EvalCondition(_condition.ToArray(), Script).HasErrored(out var error2, out condition))
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