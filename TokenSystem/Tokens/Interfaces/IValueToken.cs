using SER.Helpers.ResultSystem;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens.Interfaces;

public interface IValueToken
{
    public TryGet<Value> BaseValue { get; }
}