using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public abstract class LiteralValueToken<T> : BaseToken, IValueCapableToken<LiteralValue> 
    where T : LiteralValue 
{
    private bool _set = false;
    
#pragma warning disable CS9264
    public T Value
#pragma warning restore CS9264
    {
        get => _set ? field : throw new AndrzejFuckedUpException();
        protected set
        {
            _set = true;
            field = value;
        }
    }

    public TryGet<LiteralValue> ExactValue => Value;
    public TryGet<Value> BaseValue => Value;
}