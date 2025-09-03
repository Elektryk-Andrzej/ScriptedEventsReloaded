namespace SER.ScriptSystem.ContextSystem.BaseContexts;

public abstract class StandardContext : Context
{
    public void Run()
    {
        Script.CurrentLine = LineNum;
        Execute();
    }
    
    protected abstract void Execute();
}