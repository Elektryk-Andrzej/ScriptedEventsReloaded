using SER.ValueSystem;

namespace SER.VariableSystem.Variables;

// todo: reference variables should have their own syntax
public sealed class ReferenceVariable(string name, ReferenceValue value) 
    : TypeVariable<ReferenceValue>(name, value);