using System.Collections.Generic;
using System.Linq;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Extensions;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.Control.Loops;

public class ForeverLoopContext : StatementContext, IKeywordContext
{
    private readonly ResultStacker _rs = new("Cannot create 'forever' loop.");
    private bool _skipChild = false;
    
    public string Keyword => "forever";
    public string Description => "Makes the code inside the statement run indefinitely.";
    public string[] Arguments => [];

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Error(_rs.Add("'forever' loop doesn't expect any arguments."));
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    public override IEnumerator<float> Execute()
    {
        while (true)
        {
            foreach (var coro in Children.Select(child => child.ExecuteBaseContext()))
            {
                while (coro.MoveNext())
                {
                    yield return coro.Current;
                }

                if (!_skipChild) continue;

                _skipChild = false;
                break;
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