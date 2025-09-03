using SER.Helpers;

namespace SER.ScriptSystem.TokenSystem.Tokens.LiteralVariables;

public interface ILiteralValueSyntaxToken
{
    public TryGet<string> TryGetValue(Script scr);
}