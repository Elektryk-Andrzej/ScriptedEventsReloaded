using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Contexts.VariableDefinition;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Structures;
using SER.VariableSystem;

namespace SER.ScriptSystem.TokenSystem.Tokens.LiteralVariables;

public class LiteralVariableToken : ContextableToken, IUseBrackets, ILiteralValueSyntaxToken
{
    private int _openBrackets = 0;

    public char OpeningBracket => '{';
    public char ClosingBracket => '}';
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
        if (_openBrackets != 0) 
            return $"Variable '{RawRepresentation}' is not fully closed (open {_openBrackets}).";

        if (RawRepresentation.Length <= 2) 
            return $"Variable '{RawRepresentation}' does not have a name.";

        return true;
    }

    public override TryGet<Context> TryGetResultingContext()
    {
        return new LiteralVariableDefinitionContext(this)
        {
            Script = Script,
            LineNum = LineNum
        };
    }

    public TryGet<string> TryGetValue(Script scr)
    {
        return VariableParser.ParseValueSyntax(RawRepresentation, Script);
    }
}