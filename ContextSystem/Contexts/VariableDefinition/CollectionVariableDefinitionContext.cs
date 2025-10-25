using SER.TokenSystem.Tokens.Variables;
using SER.ValueSystem;
using SER.VariableSystem.Variables;

namespace SER.ContextSystem.Contexts.VariableDefinition;

public class CollectionVariableDefinitionContext(VariableToken<CollectionVariable, CollectionValue> varToken) : 
    VariableDefinitionContext<VariableToken<CollectionVariable, CollectionValue>, CollectionValue, CollectionVariable>(varToken);