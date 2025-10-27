using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;

namespace SER.ValueSystem;

public class ReferenceValue(object? obj) : Value
{
    public bool IsValid => Value is not null;
    public object Value => obj ?? throw new ScriptRuntimeError("Value of reference is invalid.");

    public override string ToString()
    {
        return $"<{Value.GetType().GetAccurateName()} reference>";
    }
}