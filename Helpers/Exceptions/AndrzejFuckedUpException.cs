using System;

namespace SER.Helpers.Exceptions;

public class AndrzejFuckedUpException : SystemException
{
    public AndrzejFuckedUpException()
    {
    }
    
    public AndrzejFuckedUpException(string msg) : base(msg)
    {
    }
}