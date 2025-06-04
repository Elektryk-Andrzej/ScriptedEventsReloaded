using System.Diagnostics.Contracts;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Structures;

namespace SER.ScriptSystem.TokenSystem;

public static class TokenExtensions
{
    [Pure]
    public static string GetValue(this BaseToken token)
    {
        if (token is IUseBrackets parentheses) return parentheses.ValueWithoutBrackets;

        return token.RawRepresentation;
    }
}