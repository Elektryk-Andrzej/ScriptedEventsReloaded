using System;
using JetBrains.Annotations;
using SER.Helpers.Extensions;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.Plugin.Commands.HelpSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents an enum argument used in a method.
/// </summary>
/// <typeparam name="TEnum">
/// The specific enum type this argument represents.
/// </typeparam>
public class EnumArgument<TEnum> : CustomMethodArgument where TEnum : struct, Enum
{
    public EnumArgument(string name) : base(name)
    {
        HelpInfoStorage.UsedEnums.Add(typeof(TEnum));
    }
    
    public override string InputDescription => $"{typeof(TEnum).GetAccurateName()} enum value.";

    [UsedImplicitly]
    public ArgumentEvaluation<object> GetConvertSolution(BaseToken token)
    {
        return CustomConvertSolution(token, InternalConvert);
    }

    private ArgumentEvaluation<object>.EvalRes InternalConvert(string value)
    {
        if (Enum.TryParse(value, true, out TEnum result))
        {
            return result;
        }
        
        return Rs.Add($"Value '{value}' is not a valid {typeof(TEnum).GetAccurateName()} enum value.");
    }
}