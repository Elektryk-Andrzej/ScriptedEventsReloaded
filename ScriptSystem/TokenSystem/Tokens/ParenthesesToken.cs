﻿using SER.Helpers.ResultStructure;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Structures;

namespace SER.ScriptSystem.TokenSystem.Tokens;

public class ParenthesesToken : BaseToken, IUseBrackets
{
    private int _openBrackets = 0;
    
    public char OpeningBracket => '(';
    public char ClosingBracket => ')';
    public string ValueWithoutBrackets => RawRepresentation.Substring(1, RawRepresentation.Length - 2);

    protected override void OnAddChar(char c)
    {
        if (c == OpeningBracket) _openBrackets++;
        else if (c == ClosingBracket) _openBrackets--;
    }

    public override bool EndParsingOnChar(char c, out BaseToken? replaceToken)
    {
        replaceToken = null;
        return _openBrackets == 0;
    }

    public override Result IsValidSyntax()
    {
        return Result.Assert(
            _openBrackets == 0,
            $"Parantheses '{RawRepresentation}' are not fully closed.");
    }
}