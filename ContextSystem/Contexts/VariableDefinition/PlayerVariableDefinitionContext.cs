using System;
using SER.ContextSystem.Structures;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.VariableTokens;
using SER.ValueSystem;
using SER.VariableSystem.Variables;

namespace SER.ContextSystem.Contexts.VariableDefinition;

public class PlayerVariableDefinitionContext(VariableToken<PlayerVariable, PlayerValue> varToken) : 
    VariableDefinitionContext<VariableToken<PlayerVariable, PlayerValue>, PlayerValue, PlayerVariable>(varToken)
{
    protected override (TryAddTokenRes result, Func<PlayerValue> parser) AdditionalParsing(BaseToken token)
    {
        if (token is ParenthesesToken { RawContent: "" })
        {
            return (TryAddTokenRes.End(), () => new([]));
        }

        return base.AdditionalParsing(token);
    }
}