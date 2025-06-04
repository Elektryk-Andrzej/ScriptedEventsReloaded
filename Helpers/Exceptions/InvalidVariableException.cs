using System;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.Helpers.Exceptions;

public class InvalidVariableException(PlayerVariableToken token)
    : SystemException($"Variable '{token.RawRepresentation}' does not exist!");