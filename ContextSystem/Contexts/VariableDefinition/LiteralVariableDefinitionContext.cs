using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.VariableTokens;
using SER.ValueSystem;
using SER.VariableSystem.Variables;

namespace SER.ContextSystem.Contexts.VariableDefinition;

public class LiteralVariableDefinitionContext :
    VariableDefinitionContext<VariableToken<LiteralVariable, LiteralValue>, LiteralValue, LiteralVariable>
{
    public LiteralVariableDefinitionContext(VariableToken<LiteralVariable, LiteralValue> varToken) : base(varToken)
    {
        AdditionalTokenParser = token =>
        {
            if (token is TextToken textToken)
            {
                return () => new TextValue(textToken.ParsedValue());
            }

            return null;
        };
    }
}


