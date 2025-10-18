using SER.Helpers.Exceptions;
using SER.ValueSystem;

namespace SER.VariableSystem.Variables;

public abstract class LiteralVariable : IVariable
{
    public abstract string Name { get; }
    public abstract LiteralValue BaseValue { get; }
    
    public static LiteralVariable CopyVariable(LiteralVariable variable)
    {
        var name = variable.Name;
        return variable switch
        {
            TextVariable text => new TextVariable(name, new(text.ExactValue)),
            NumberVariable number => new NumberVariable(name, new(number.ExactValue)),
            BoolVariable @bool => new BoolVariable(name, new(@bool.ExactValue)),
            ReferenceVariable @ref => new ReferenceVariable(name, @ref.ExactValue),
            DurationVariable dur => new DurationVariable(name, dur.ExactValue),
            _ => throw new AndrzejFuckedUpException(
                $"CopyVariable called on variable of type {variable.GetType().Name}")
        };
    }

    public static LiteralVariable CreateVariable(string name, LiteralValue value)
    {
        return value switch
        {
            BoolValue @bool => new BoolVariable(name, @bool),
            DurationValue dur => new DurationVariable(name, dur),
            NumberValue num => new NumberVariable(name, num),
            ReferenceValue @ref => new ReferenceVariable(name, @ref),
            TextValue text => new TextVariable(name, text),
            _ => throw new AndrzejFuckedUpException(
                $"CreateVariable called on invalid value type {value.GetType().Name}")
        };
    }
}