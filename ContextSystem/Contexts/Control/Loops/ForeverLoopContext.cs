using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Extensions;
using SER.ContextSystem.Structures;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ContextSystem.Contexts.Control.Loops;

[UsedImplicitly]
public class ForeverLoopContext : StatementContext, IKeywordContext
{
    private readonly Result _mainErr = "Cannot create 'forever' loop.";
    private bool _skipChild = false;
    
    public string KeywordName => "forever";
    public string Description => "Makes the code inside the statement run indefinitely.";
    public string[] Arguments => [];

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Error(_mainErr + "'forever' loop doesn't expect any arguments.");
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    protected override IEnumerator<float> Execute()
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