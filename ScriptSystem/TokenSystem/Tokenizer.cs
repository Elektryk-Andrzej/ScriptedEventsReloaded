using System.Collections.Generic;
using SER.Helpers;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Structures;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.ScriptSystem.TokenSystem;

public class Tokenizer(Script script)
{
    public void GetAllFileTokens()
    {
        List<ScriptLine> tokens = [];
        for (var index = 0; index < script.Content.Split('\n').Length; index++)
        {
            var line = script.Content.Split('\n')[index];
            tokens.Add(GetTokensFromLine(line, index + 1));
        }

        script.Tokens = tokens;
    }

    public ScriptLine GetTokensFromLine(string lineContent, int lineNum)
    {
        return GetTokensFromLine(lineContent.ToCharArray(), lineNum);
    }

    public ScriptLine GetTokensFromLine(char[] lineContent, int lineNum, BaseToken? initialLexer = null)
    {
        var currentToken = initialLexer;
        List<BaseToken> tokens = [];

        foreach (var currentChar in lineContent)
        {
            if (currentToken is null)
            {
                currentToken = GetLexer(currentChar, lineNum);

                if (currentToken is not null)
                {
                    Log.Debug($"Set new token lexer to: {currentToken}");
                    currentToken.AddChar(currentChar);
                }

                continue;
            }
            
            if (!currentToken.EndParsingOnChar(currentChar))
            {
                currentToken.AddChar(currentChar);
                continue;
            }
            
            if (!char.IsWhiteSpace(currentChar))
            {
                Log.Warn(script, $"Expected whitespace after {currentToken}, got character '{currentChar}' instead; converting to unclassified value");
                currentToken = new UnclassifiedValueToken(currentToken.RawRepresentation)
                {
                    Script = script,
                    LineNum = currentToken.LineNum,
                };
            }

            AddToken();
        }

        if (currentToken is not null)
        {
            AddToken();
        }

        return new()
        {
            LineNumber = lineNum,
            Script = script,
            Tokens = tokens
        };

        void AddToken()
        {
            if (currentToken.IsValidSyntax().HasErrored(out var msg))
            {
                Log.Error(script, $"Token {currentToken} has failed: '{msg}'; converting to unclassified value");
                currentToken = new UnclassifiedValueToken(currentToken.RawRepresentation) 
                {
                    Script = script,
                    LineNum = lineNum
                };
            }

            Log.Debug($"Token lexer {currentToken} has stopped");
            tokens.Add(currentToken);
            currentToken = null;
        }
    }

    // if C# allowed for static override this wouldve been unnecessary
    private BaseToken? GetLexer(char character, int lineNum)
    {
        // whitespaces are ignored
        if (char.IsWhiteSpace(character)) return null;

        switch (character)
        {
            case '#':
                return new CommentToken
                {
                    Script = script,
                    LineNum = lineNum
                };
            case '!':
                return new FlagToken
                {
                    Script = script,
                    LineNum = lineNum
                };
            case '@':
                return new PlayerVariableToken
                {
                    Script = script,
                    LineNum = lineNum
                };
            case '{':
                return new LiteralVariableToken
                {
                    Script = script,
                    LineNum = lineNum
                };
            case '(':
                return new ParenthesesToken
                {
                    Script = script,
                    LineNum = lineNum
                };
        }

        if (char.IsUpper(character)) return new MethodToken
        {
            Script = script,
            LineNum = lineNum
        };
        
        if (char.IsLower(character)) return new KeywordToken
        {
            Script = script,
            LineNum = lineNum
        };
        
        return new UnclassifiedValueToken
        {
            Script = script,
            LineNum = lineNum
        };
    }
}