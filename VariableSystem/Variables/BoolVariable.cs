using SER.ValueSystem;

namespace SER.VariableSystem.Variables;

public class BoolVariable(string name, BoolValue value) 
    : TypeVariable<BoolValue>(name, value);