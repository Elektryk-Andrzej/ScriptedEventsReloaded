using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ValueSystem;

namespace SER.TokenSystem.Structures;

public interface ILiteralValueToken
{
    public TryGet<LiteralValue> GetLiteralValue(Script scr);
}