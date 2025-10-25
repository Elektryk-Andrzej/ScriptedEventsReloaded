using SER.ValueSystem;
using SER.VariableSystem.Bases;

namespace SER.VariableSystem.Variables;

public class ReferenceVariable(string name, ReferenceValue value) : Variable<ReferenceValue>
{
    public override string Name => name;
    public override ReferenceValue Value => value;
}