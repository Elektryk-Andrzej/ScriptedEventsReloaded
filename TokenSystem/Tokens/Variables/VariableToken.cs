using System.Linq;
using SER.ContextSystem.BaseContexts;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Structures;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;
using SER.VariableSystem.Bases;

namespace SER.TokenSystem.Tokens.Variables;

public abstract class VariableToken : BaseToken, IContextableToken
{
    public abstract char Prefix { get; }
    public abstract string Name { get; protected set; }
    
    public TryGet<Variable> TryGetVariable()
    {
        return Script.TryGetVariable<Variable>(this);
    }

    public abstract TryGet<Context> TryGetContext(Script scr);
}

public abstract class VariableToken<TVariable, TValue> : VariableToken, IValueCapableToken<TValue>
    where TVariable : Variable<TValue>
    where TValue : Value
{
    public override string Name { get; protected set; } = null!;

    public TryGet<Value> BaseValue => TryGetVariable().OnSuccess(Value (var) => var.Value);

    public new TryGet<TVariable> TryGetVariable()
    {
        return Script.TryGetVariable<TVariable>(this);
    }

    public TryGet<TValue> ExactValue => TryGetVariable().OnSuccess(variable => variable.Value);

    protected override Result InternalParse(Script scr)
    {
        if (RawRep.Length < 2)
        {
            return "Variable name is too short.";
        }

        if (RawRep.FirstOrDefault() != Prefix)
        {
            return $"Variable is missing '{Prefix}' prefix.";
        }
        
        Name = RawRep.Substring(1);
        if (Name.Any(c => !char.IsLetter(c) && !char.IsDigit(c) && c != '_'))
        {
            return "Variable name can only contain letters, digits and underscores.";
        }
        
        return true;
    }
}