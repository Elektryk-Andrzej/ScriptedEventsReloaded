using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.ValueSystem;
using SER.VariableSystem.Variables;

namespace SER.VariableSystem.Bases;

public abstract class Variable
{
    public abstract string Name { get; }

    public abstract Value BaseValue { get; }

    public static Variable CopyVariable(Variable variable)
    {
        var name = variable.Name;
        return variable switch
        {
            CollectionVariable coll => new CollectionVariable(name, new(coll.Value.CastedValues)),
            PlayerVariable plr      => new PlayerVariable(name, new(plr.Players)),
            ReferenceVariable @ref  => @ref,
            LiteralVariable lit     => new LiteralVariable(name, lit.Value),
            _ => throw new AndrzejFuckedUpException(
                $"CopyVariable called on variable of type {variable.GetType().AccurateName}")
        };
    }

    public static Variable CreateVariable(string name, Value value)
    {
        return value switch
        {
            LiteralValue lit     => new LiteralVariable(name, lit),
            CollectionValue coll => new CollectionVariable(name, coll),
            PlayerValue plr      => new PlayerVariable(name, plr),
            ReferenceValue @ref  => new ReferenceVariable(name, @ref),
            _ => throw new AndrzejFuckedUpException(
                $"CreateVariable called on invalid value type {value.GetType().AccurateName}")
        };
    }
}

public abstract class Variable<TValue> : Variable
    where TValue : Value
{
    public abstract TValue Value { get; }
    public override Value BaseValue => Value;
}