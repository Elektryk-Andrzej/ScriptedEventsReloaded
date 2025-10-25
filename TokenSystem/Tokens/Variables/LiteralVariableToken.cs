using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Contexts.VariableDefinition;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ValueSystem;
using SER.VariableSystem.Variables;

namespace SER.TokenSystem.Tokens.Variables;

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
}