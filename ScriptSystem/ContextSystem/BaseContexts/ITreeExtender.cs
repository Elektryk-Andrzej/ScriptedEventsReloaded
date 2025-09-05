using System;
using SER.Helpers;

namespace SER.ScriptSystem.ContextSystem.BaseContexts;

public interface ITreeExtender
{
    public abstract IExtendableTree.ControlMessage Extends { get; }
}