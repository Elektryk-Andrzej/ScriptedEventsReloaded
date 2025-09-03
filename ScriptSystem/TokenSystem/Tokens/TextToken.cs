using SER.Helpers.ResultStructure;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Structures;

namespace SER.ScriptSystem.TokenSystem.Tokens;

public class TextToken : BaseToken, IUseBrackets
{
    private bool _ignoreNextChar = false;
    private bool _finished = false;
    
    public char OpeningBracket => '"';
    public char ClosingBracket => '"';
    public string ValueWithoutBrackets => RawRepresentation.Substring(1, RawRepresentation.Length - 2);

    protected override void OnAddChar(char c)
    {
        if (_ignoreNextChar)
        {
            RawRepresentation = RawRepresentation.Substring(0, RawRepresentation.Length - 1);
        }
    }
    
    public override bool EndParsingOnChar(char c, out BaseToken? replaceToken)
    {
        replaceToken = null;
        if (_finished) return true;
        
        if (_ignoreNextChar)
        {
            _ignoreNextChar = false;
            return false;
        }
        
        switch (c)
        {
            case '\\':
                _ignoreNextChar = true;
                return false;
            case '"':
                _finished = true;
                return false;
            default:
                return false;
        }
    }

    public override Result IsValidSyntax()
    {
        return Result.Assert(
            _finished,
            $"{RawRepresentation} <- missing \" character to end the text");
    }
}