using System;

namespace SER.Helpers.Exceptions;

public class InvalidValueException(string expectedValue, string valueReceived)
    : SystemException(
        $"Expected value '{expectedValue}', but received '{valueReceived}', which doesn't match.");