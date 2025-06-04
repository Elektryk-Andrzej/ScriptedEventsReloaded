using System;

namespace SER.Helpers.Exceptions;

public class DeveloperFuckupException : SystemException
{
    public DeveloperFuckupException()
    {
    }
    
    public DeveloperFuckupException(string msg) : base(msg)
    {
    }
}