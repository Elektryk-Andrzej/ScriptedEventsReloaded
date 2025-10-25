using SER.Helpers.ResultSystem;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens.Interfaces;

public interface IValueCapableToken<T> : IValueToken
    where T : Value
{
    public TryGet<T> ExactValue { get; }
}