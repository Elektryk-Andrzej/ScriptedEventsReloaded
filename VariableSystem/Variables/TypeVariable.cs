using SER.ValueSystem;

namespace SER.VariableSystem.Variables;

public abstract class TypeVariable<T>(string name, T value) : LiteralVariable
    where T : LiteralValue
{
    public override string Name => name;
    public T ExactValue => value;
    public override LiteralValue BaseValue => value;
}