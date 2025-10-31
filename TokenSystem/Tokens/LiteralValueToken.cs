using System;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public abstract class LiteralValueToken<T> : BaseToken, IValueToken
    where T : LiteralValue 
{
    private bool _set = false;
    public T Value
    {
        get => _set ? field : throw new AndrzejFuckedUpException($"Value of a {GetType().AccurateName} was not set.");
        protected set
        {
            _set = true;
            field = value;
        }
    } = null!;

    public TryGet<LiteralValue> ExactValue => Value;
    TryGet<Value> IValueToken.Value() => Value;
    public Type[] PossibleValueTypes => [typeof(T)];
    public bool IsConstant => true;
}