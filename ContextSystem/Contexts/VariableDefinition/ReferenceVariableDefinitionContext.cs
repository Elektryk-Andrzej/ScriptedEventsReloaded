using SER.TokenSystem.Tokens.VariableTokens;
using SER.ValueSystem;
using SER.VariableSystem.Variables;

namespace SER.ContextSystem.Contexts.VariableDefinition;

public class ReferenceVariableDefinitionContext(VariableToken<ReferenceVariable, ReferenceValue> varToken) : 
    VariableDefinitionContext<VariableToken<ReferenceVariable, ReferenceValue>, ReferenceValue, ReferenceVariable>(varToken);