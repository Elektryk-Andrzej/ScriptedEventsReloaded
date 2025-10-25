using System;
using System.Collections.Generic;
using SER.ContextSystem.Structures;

namespace SER.ContextSystem.BaseContexts;

public abstract class LoopContext : StatementContext, IExtendableStatement, IKeywordContext
{
    public IExtendableStatement.Signal AllowedSignals => IExtendableStatement.Signal.DidntExecute;
    public abstract Dictionary<IExtendableStatement.Signal, Func<IEnumerator<float>>> RegisteredSignals { get; }
    
    public abstract string KeywordName { get; }
    public abstract string Description { get; }
    public abstract string[] Arguments { get; }
    
    public bool SkipThisIteration { get; protected set; }
    public bool ExitLoop { get; protected set; }

    protected override void OnReceivedControlMessageFromChild(ParentContextControlMessage msg)
    {
        switch (msg)
        {
            case ParentContextControlMessage.LoopContinue:
                SkipThisIteration = true;
                return;
            case ParentContextControlMessage.LoopBreak:
                ExitLoop = true;
                return;
            default:
                ParentContext?.SendControlMessage(msg);
                break;
        }
    }
}