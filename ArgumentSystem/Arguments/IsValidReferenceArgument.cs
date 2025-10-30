using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.ValueSystem;

namespace SER.ArgumentSystem.Arguments;

public class IsValidReferenceArgument(string name) : Argument(name)
{
    public override string InputDescription => "a reference we want to check is valid e.g. @room";
    
    [UsedImplicitly]
    public DynamicTryGet<bool> GetConvertSolution(BaseToken token)
    {
        if (!token.CanReturn<ReferenceValue>(out var func))
        {
            return $"Value '{token.RawRep}' is not a reference.";
        }
        
        return new(() => func().OnSuccess(v => v.IsValid));
    }
}