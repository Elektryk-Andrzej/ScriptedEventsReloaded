using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Contexts.VariableDefinition;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ValueSystem;
using SER.VariableSystem.Variables;

namespace SER.TokenSystem.Tokens.Variables;

public class PlayerVariableToken : VariableToken<PlayerVariable, PlayerValue>
{
    public override char Prefix => '@';

    public static string Example => "@players";

    public override TryGet<Context> TryGetContext(Script scr)
    {
        return new PlayerVariableDefinitionContext(this)
        {
            Script = scr,
            LineNum = LineNum,
        };
    }
}