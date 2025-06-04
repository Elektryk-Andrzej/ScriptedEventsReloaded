using System;

namespace SER.Helpers.Exceptions;

public class ArgumentFetchException(string msg) : SystemException(msg);