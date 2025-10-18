using System.Collections.Generic;

namespace SER.ContextSystem.BaseContexts;

public abstract class YieldingContext : Context
{
    public IEnumerator<float> Run()
    {
        if (LineNum.HasValue)
        {
            Script.CurrentLine = LineNum.Value;
        }
        
        var enumerator = Execute();
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    protected abstract IEnumerator<float> Execute();
}