using System;
using System.Linq;
using SER.ContextSystem.BaseContexts;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Structures;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;
using SER.VariableSystem.Bases;

namespace SER.TokenSystem.Tokens.VariableTokens;

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

public abstract class VariableToken<TVariable, TValue> : VariableToken, IValueToken
    where TVariable : Variable<TValue>
    where TValue : Value
{
    public override string Name { get; protected set; } = null!;
    
    public new TryGet<TVariable> TryGetVariable()
    {
        return Script.TryGetVariable<TVariable>(this);
    }

    public TryGet<TValue> ExactValue => TryGetVariable().OnSuccess(variable => variable.Value);

    protected override IParseResult InternalParse(Script scr)
    {
        if (RawRep.Length < 2 || RawRep.FirstOrDefault() != Prefix)
        {
            return new Ignore();
        }

        Name = RawRep.Substring(1);
        if (Name.Any(c => !char.IsLetter(c) && !char.IsDigit(c) && c != '_'))
        {
            return new Ignore();
        }
        
        return new Success();
    }

    public TryGet<Value> Value()
    {
        return TryGetVariable().OnSuccess(Value (variable) => variable.Value);
    }

    public Type[]? PossibleValueTypes => [typeof(TValue)];
    public bool IsConstant => false;
}