﻿using System.Diagnostics.Contracts;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.ScriptSystem.TokenSystem;

public static class TokenExtensions
{
    [Pure]
    public static string GetValue(this BaseToken token)
    {
        if (token is TextToken text) 
            return text.ValueWithoutBrackets;

        return token.RawRepresentation;
    }
}