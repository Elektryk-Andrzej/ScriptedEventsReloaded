using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SER.ScriptSystem.ContextSystem.Structures;

public interface IExtendableStatement
{
    [Flags]
    public enum Signal
    {
        [UsedImplicitly] 
        None         = 0,
        DidntExecute = 1 << 0,
    }

    public abstract Signal AllowedSignals { get; }
    public Dictionary<Signal, Func<IEnumerator<float>>> RegisteredSignals { get; }
}