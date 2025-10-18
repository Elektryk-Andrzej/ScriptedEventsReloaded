namespace SER.ContextSystem.BaseContexts;

public abstract class StandardContext : Context
{
    public void Run()
    {
        if (LineNum.HasValue)
        {
            Script.CurrentLine = LineNum.Value;
        }
        
        Execute();
    }
    
    protected abstract void Execute();
}