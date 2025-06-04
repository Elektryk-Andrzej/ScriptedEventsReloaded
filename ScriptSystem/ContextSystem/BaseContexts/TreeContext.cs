using System.Collections.Generic;
using SER.Helpers;
using SER.ScriptSystem.ContextSystem.Structures;

namespace SER.ScriptSystem.ContextSystem.BaseContexts;

public abstract class TreeContext : YieldingContext
{
    public readonly List<BaseContext> Children = [];
    
    public void SendControlMessage(ParentContextControlMessage msg)
    {
        Log.Debug($"{Name} context has received control message: {msg}");
        OnReceivedControlMessageFromChild(msg);
    }

    protected abstract void OnReceivedControlMessageFromChild(ParentContextControlMessage msg);
}