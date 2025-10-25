using SER.ValueSystem;
using SER.VariableSystem.Bases;

namespace SER.VariableSystem.Variables;

public class CollectionVariable(string name, CollectionValue value) : Variable<CollectionValue>
{
    public override string Name => name;
    public override CollectionValue Value => value;
}