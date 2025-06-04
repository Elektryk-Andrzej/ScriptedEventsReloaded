using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.BaseContexts;

public abstract class BaseContext
{
    public string Name => GetType().Name;
    public Script Script { get; init; } = null!;
    public int LineNum { get; init; }

    public TreeContext? ParentContext { get; set; } = null;

    public abstract TryAddTokenRes TryAddToken(BaseToken token);

    public override string ToString()
    {
        return Name;
    }

    public abstract Result VerifyCurrentState();
    
    public static BaseContext Create<TContext>((Script scr, int lineNum) info) 
        where TContext : BaseContext, new()
    {
        return new TContext
        {
            Script = info.scr,
            LineNum = info.lineNum
        };
    }
}