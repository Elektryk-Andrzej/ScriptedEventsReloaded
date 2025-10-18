using SER.ValueSystem;

namespace SER.VariableSystem.Variables;

public sealed class TextVariable(string name, TextValue textValue) 
    : TypeVariable<TextValue>(name, textValue);