using System.Diagnostics.Contracts;
using SER.Helpers.ResultStructure;

namespace SER.ScriptSystem.TokenSystem.BaseTokens;

public abstract class BaseToken(string initRep = "")
{
    public string TokenName => GetType().Name;
    public string RawRepresentation { get; protected set; } = initRep;
    public required Script Script { get; init; }
    public required int LineNum { get; init; }

    public void AddChar(char c)
    {
        OnAddingChar(c);
        RawRepresentation += c;
    }

    [Pure]
    public override string ToString()
    {
        return RawRepresentation.Length > 0
            ? $"{TokenName} (value: '{RawRepresentation}')"
            : TokenName;
    }

    [Pure]
    public abstract bool EndParsingOnChar(char c);

    protected virtual void OnAddingChar(char c)
    {
    }

    [Pure]
    public abstract Result IsValidSyntax();
}