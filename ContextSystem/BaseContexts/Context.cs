using System;
using SER.ContextSystem.Structures;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Tokens;

namespace SER.ContextSystem.BaseContexts;

public abstract class Context
{
    public string Name => GetType().Name;
    
    public required Script Script { get; set; } = null!;
    
    public required uint? LineNum { get; set; }

    public StatementContext? ParentContext { get; set; } = null;

    public abstract TryAddTokenRes TryAddToken(BaseToken token);

    public abstract Result VerifyCurrentState();

    public static Context Create(Type contextType, (Script scr, uint? lineNum) info)
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