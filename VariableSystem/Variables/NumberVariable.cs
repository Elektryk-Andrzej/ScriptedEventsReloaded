using SER.ValueSystem;

namespace SER.VariableSystem.Variables;

public sealed class NumberVariable(string name, NumberValue value) 
    : TypeVariable<NumberValue>(name, value);