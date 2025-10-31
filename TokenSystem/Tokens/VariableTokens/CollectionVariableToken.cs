using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Contexts.VariableDefinition;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ValueSystem;
using SER.VariableSystem.Variables;

namespace SER.TokenSystem.Tokens.VariableTokens;

public class CollectionVariableToken : VariableToken<CollectionVariable, CollectionValue>
{
    public override char Prefix => '&';

    public static string Example => "&collection";

    public override TryGet<Context> TryGetContext(Script scr)
    {
        return new CollectionVariableDefinitionContext(this)
        {
            Script = scr,
            LineNum = LineNum,
        };
    }
}