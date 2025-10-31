using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Contexts.VariableDefinition;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ValueSystem;
using SER.VariableSystem.Variables;

namespace SER.TokenSystem.Tokens.VariableTokens;

public class LiteralVariableToken : VariableToken<LiteralVariable, LiteralValue>
{
    public override char Prefix => '$';
    
    public override TryGet<Context> TryGetContext(Script scr)
    {
        return new LiteralVariableDefinitionContext(this)
        {
            Script = scr,
            LineNum = LineNum,
        };
    }
    
    public TryGet<T> TryGetValue<T>() where T : Value
    {
        if (TryGetVariable().HasErrored(out var varError, out var variable))
        {
            return varError;
        }

        if (variable.TryGetValue<T>().HasErrored(out var valError, out var value))
        {
            return valError;
        }
                
        return value;
    }
}