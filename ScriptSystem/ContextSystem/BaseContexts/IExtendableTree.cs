using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SER.Helpers;
using SER.ScriptSystem.ContextSystem.Structures;

namespace SER.ScriptSystem.ContextSystem.BaseContexts;

public interface IExtendableTree
{
    [Flags]
    public enum ControlMessage
    {
        [UsedImplicitly] 
        None         = 0,
        DidntExecute = 1 << 0,
    }

    public abstract ControlMessage AllowedControlMessages { get; }
    public Dictionary<ControlMessage, Func<IEnumerator<float>>> ControlMessages { get; }
}