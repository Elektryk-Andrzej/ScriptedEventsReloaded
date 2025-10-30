using System.Text;
using SER.Helpers.ResultSystem;

namespace SER.TokenSystem.Slices;

public abstract class Slice(char startChar)
{
    public string RawRep => PrivateRawRepresentation.ToString();
    protected StringBuilder PrivateRawRepresentation { get; } = new(startChar.ToString());

    public abstract string Value { get; }

    public abstract bool CanContinueAfterAdd(char c);

    public abstract Result VerifyState();
}