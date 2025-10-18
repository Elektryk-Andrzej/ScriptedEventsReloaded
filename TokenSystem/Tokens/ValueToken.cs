using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public abstract class ValueToken<T> : BaseToken where T : LiteralValue 
{
    private bool _set = false;
    
#pragma warning disable CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or adding '[field: MaybeNull, AllowNull]' attributes.
    public T Value
#pragma warning restore CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or adding '[field: MaybeNull, AllowNull]' attributes.
    {
        get => _set ? field : throw new AndrzejFuckedUpException();
        protected set
        {
            _set = true;
            field = value;
        }
    }
    
    public TryGet<LiteralValue> GetLiteralValue(Script scr)
    {
        return Value;
    }
}