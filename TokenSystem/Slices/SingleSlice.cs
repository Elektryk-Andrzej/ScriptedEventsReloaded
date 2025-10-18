using SER.Helpers.ResultSystem;

namespace SER.TokenSystem.Slices;

public class SingleSlice(char startChar) : Slice(startChar)
{
    public override string Value => RawRepresentation;
    
    public override bool CanContinueAfterAdd(char c)
    {
        if (char.IsWhiteSpace(c)) return false;
        PrivateRawRepresentation.Append(c);
        return true;
    }

    public override Result VerifyState()
    {
        return true;
    }
}