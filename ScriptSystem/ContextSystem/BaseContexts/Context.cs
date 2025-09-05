using System;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.BaseContexts;

public abstract class Context
{
    public string Name => GetType().Name;
    
    public Script Script { get; set; } = null!;
    
    public int LineNum { get; set; }

    public StatementContext? ParentContext { get; set; } = null;

    public abstract TryAddTokenRes TryAddToken(BaseToken token);

    public abstract Result VerifyCurrentState();

    public static Context Create(Type contextType, (Script scr, int lineNum) info)
    {
        var context = (Context)Activator.CreateInstance(contextType);
        context.Script = info.scr;
        context.LineNum = info.lineNum;
        return context;
    }

    public override string ToString()
    {
        return Name;
    }
}