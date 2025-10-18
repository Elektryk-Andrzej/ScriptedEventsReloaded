using System;

namespace SER.Helpers.Exceptions;

public class ScriptErrorException(string error) : SystemException(error);