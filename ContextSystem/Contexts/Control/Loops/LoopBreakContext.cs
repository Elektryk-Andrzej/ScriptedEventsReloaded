﻿using JetBrains.Annotations;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Structures;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ContextSystem.Contexts.Control.Loops;

[UsedImplicitly]
public class LoopBreakContext : StandardContext, IKeywordContext
{
    public string KeywordName => "break";
    public string Description => "Makes a given loop (that the 'break' keyword is inside) act as it has completely " +
                                 "ended its execution (\"breaks\" free from the loop)";
    public string[] Arguments => [];
    
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Error("The 'break' keyword does not expect arguments after it.");
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    protected override void Execute()
    {
        ParentContext?.SendControlMessage(ParentContextControlMessage.LoopBreak);
    }
}