using System;
using LabApi.Features.Enums;
using MapGeneration;
using SER.Helpers.ResultStructure;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.Plugin.HelpSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents an enum argument used in a method.
/// </summary>
/// <typeparam name="TEnum">
/// The specific enum type this argument represents.
/// </typeparam>
public class EnumArgument<TEnum> : BaseMethodArgument where TEnum : struct, Enum
{
    public EnumArgument(string name) : base(name)
    {
        HelpInfoStorage.UsedEnums.Add(typeof(TEnum));
    }
    
    public override OperatingValue Input
    {
        get
        {
            if (typeof(TEnum) == typeof(RoomName))
            {
                return OperatingValue.RoomName;
            }
            if (typeof(TEnum) == typeof(FacilityZone))
            {
                return OperatingValue.FacilityZone;
            }
            if (typeof(TEnum) == typeof(DoorName))
            {
                return OperatingValue.DoorName;
            }

            return OperatingValue.CustomEnum;
        }
    }

    public override string AdditionalDescription => 
        $"This argument is expecting {typeof(TEnum).GetAccurateName()} enum value.";
    
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