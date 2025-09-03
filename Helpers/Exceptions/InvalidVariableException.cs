using System;
using SER.ScriptSystem.TokenSystem.Tokens;
using SER.ScriptSystem.TokenSystem.Tokens.LiteralVariables;

namespace SER.Helpers.Exceptions;

public class InvalidVariableException : SystemException
{
    public InvalidVariableException(LiteralVariableToken token) : base($"Variable '{token.RawRepresentation}' does not exist!")
    {
    }
    
    public InvalidVariableException(PlayerVariableToken token) : base($"Variable '{token.RawRepresentation}' does not exist!")
    {
    }
}