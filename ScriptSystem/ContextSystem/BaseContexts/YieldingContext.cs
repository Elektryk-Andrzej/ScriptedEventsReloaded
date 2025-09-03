using System.Collections.Generic;

namespace SER.ScriptSystem.ContextSystem.BaseContexts;

public abstract class YieldingContext : Context
{
    public IEnumerator<float> Run()
    {
        Script.CurrentLine = LineNum;
        
        var enumerator = Execute();
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
    
    protected abstract IEnumerator<float> Execute();
}