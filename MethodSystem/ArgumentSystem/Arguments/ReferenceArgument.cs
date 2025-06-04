using SER.Helpers.Extensions;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.VariableSystem;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents an argument used in a method for a reference to a specific type.
/// <remarks>
/// Do not use this argument for values that are already covered by existing method arguments.
/// </remarks>
/// </summary>
/// <typeparam name="TValue">The type of the reference being held by this argument.</typeparam>
public class ReferenceArgument<TValue>(string name) : BaseMethodArgument(name)
{
    public override OperatingValue Input => OperatingValue.CustomReference;
    public override string AdditionalDescription => 
        $"A reference to {typeof(TValue).GetAccurateName()} object.";
    
    public ArgumentEvaluation<TValue> GetConvertSolution(BaseToken token)
    {
        return CustomConvertSolution(token, InternalConvert);
    }

    private ArgumentEvaluation<TValue>.EvalRes InternalConvert(string value)
    {
        if (ObjectReferenceSystem.TryRetreiveObject(value).HasErrored(out var error, out var obj))
        {
            return Rs.Add(error);
        }
        
        return obj switch
        {
            TValue valueObj => valueObj,
            _ => $"Reference '{value}' is not a reference to {typeof(TValue).GetAccurateName()}."
        };
    }
}