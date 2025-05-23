using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a boolean argument used in a method.
/// </summary>
public class BoolArgument(string name) : BaseMethodArgument(name)
{
    public override OperatingValue Input => OperatingValue.Boolean;

    public override string AdditionalDescription =>
        "Instad of just writing 'true' or 'false', you might also consider using a condition. " +
        "Condition is a statement that checks whether something is true or false. If you have a condition like " +
        "({AmountOf *} > 5), then it will be true when there are more than 5 people on the server, otherwise false. " +
        "You can do more advanced conditions by using || and && symbols, where || means OR and && means AND, using ! " +
        "to reverse a statement etc. If you wish to learn more, see https://www.youtube.com/watch?v=HQ3dCWjfRZ4 as " +
        "pretty much everything in this video regarding conditions is valid in SER.";

    public ArgumentEvaluation<bool> GetConvertSolution(BaseToken token)
    {
        return DefaultConvertSolution<bool>(token, null);
    }
}