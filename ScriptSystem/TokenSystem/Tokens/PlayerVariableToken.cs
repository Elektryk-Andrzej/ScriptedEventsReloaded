using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Contexts.VariableDefinition;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens.LiteralVariables;

namespace SER.ScriptSystem.TokenSystem.Tokens;

public class PlayerVariableToken : ContextableToken
{
    public string NameWithoutPrefix => RawRepresentation.Substring(1);

    public override bool EndParsingOnChar(char c, out BaseToken? replaceToken)
    {
        replaceToken = null;
        if (c != '.') return !char.IsLetterOrDigit(c);
        
        replaceToken = new PlayerPropertyAccessToken(RawRepresentation + '.')
        {
            Script = Script,
            LineNum = LineNum
        };
        
        return true;
    }

    public override Result IsValidSyntax()
    {
        return Result.Assert(RawRepresentation.Length > 1,
            $"Player variable must have have the the prefix '@' and a name, " +
            $"but '{RawRepresentation}' does not satisfy that.");
    }

    public override TryGet<Context> TryGetResultingContext()
    {
        return new PlayerVariableDefinitionContext(this)
        {
            Script = Script,
            LineNum = LineNum
        };
    }
}