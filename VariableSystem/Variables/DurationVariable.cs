using SER.ValueSystem;

namespace SER.VariableSystem.Variables;

public class DurationVariable(string name, DurationValue value) 
    : TypeVariable<DurationValue>(name, value);