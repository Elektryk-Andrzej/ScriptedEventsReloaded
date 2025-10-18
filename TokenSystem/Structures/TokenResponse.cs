namespace SER.TokenSystem.Structures;

public enum TokenResponse
{
    Default,
    
    /// <summary>
    /// Char can be a valid part of the token.
    /// </summary>
    AddChar,
    
    /// <summary>
    /// Char is not supposed to appear during parsing.
    /// </summary>
    UnexpectedChar,
    
    /// <summary>
    /// Char ends token parsing.
    /// </summary>
    EndParsing,
    
    /// <summary>
    /// Char invalidates the current token, but other token will be created.
    /// </summary>
    TransformToken
}