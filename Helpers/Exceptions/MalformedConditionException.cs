using System;

namespace SER.Helpers.Exceptions;

public class MalformedConditionException(string msg) : SystemException(msg);