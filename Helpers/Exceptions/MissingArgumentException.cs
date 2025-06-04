using System;

namespace SER.Helpers.Exceptions;

public class MissingArgumentException(string msg) : SystemException(msg);