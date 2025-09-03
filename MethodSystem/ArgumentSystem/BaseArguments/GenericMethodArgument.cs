using System;
using System.Collections.Generic;
using System.Linq;
using Interactables.Interobjects;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultStructure;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.VariableSystem;
using UnityEngine;

namespace SER.MethodSystem.ArgumentSystem.BaseArguments;

public abstract class GenericMethodArgument(string name)
{
    public string Name { get; } = name;
    
    public bool ConsumesRemainingValues { get; init; } = false;
    
    public string? Description { get; init; } = null;
    
    public bool IsOptional { get; private set; } = false;

    private readonly object? _defaultValue;
    public object? DefaultValue
    {
        get => _defaultValue;
        init
        {
            IsOptional = true;
            _defaultValue = value;
        }
    }

    public OperatingValue Input { get; private set; } = 0;
    
    public abstract string? AdditionalDescription { get; }

    public Script Script { get; set; } = null!;
    
    public string GetExpectedValues()
    {
        // execute `public ArgumentEvaluation<Elevator[]> GetConvertSolution(BaseToken token)` method
        // the only reason for this is to get keys from a dictionary to get assigned to the Input property (if defined)
        try
        {
            GetType().GetMethod("GetConvertSolution")?.Invoke(this, [null]);
        }
        catch
        {
            // ignored
        }

        var values = (
            from flag in Enum.GetValues(typeof(OperatingValue))
                .Cast<OperatingValue>().
                Where(e => e is not OperatingValue.Custom)
            where Input.HasFlag(flag)
            select flag switch
            {
                OperatingValue.DoorName => $"{nameof(DoorName)} enum value",
                OperatingValue.RoomName => $"{nameof(RoomName)} enum value",
                OperatingValue.FacilityZone => $"{nameof(FacilityZone)} enum value",
                //OperatingValue.DoorReferences => $"reference to {typeof(IEnumerable<Door>).GetAccurateName()} object",
                OperatingValue.DoorReference => $"reference to a single {typeof(Door).GetAccurateName()} object",
                //OperatingValue.RoomReferences => $"reference to {typeof(IEnumerable<Room>).GetAccurateName()} object",
                OperatingValue.RoomReference => $"reference to a single {typeof(Room).GetAccurateName()} object",
                OperatingValue.AllOfType => "* character (gets all)",
                OperatingValue.Boolean => "true/false value or a condition",
                OperatingValue.Color => "color e.g. #F2137F",
                OperatingValue.Text => "any text e.g. (Never Gonna Give You Up)",
                OperatingValue.Duration => "duration e.g. 5.3s",
                OperatingValue.Float => "rational number e.g. 6.9",
                OperatingValue.Int => "integer number e.g. 420",
                OperatingValue.Percentage => "percentage e.g. 37%",
                OperatingValue.Script => "script",
                OperatingValue.Variable => "any variable",
                OperatingValue.PlayerVariableName => "player variable name",
                OperatingValue.ItemType => $"{nameof(ItemType)} enum value",
                OperatingValue.ItemReference => $"reference to {typeof(Item).GetAccurateName()} object",
                //OperatingValue.ItemReferences => $"reference to {typeof(IEnumerable<Item>).GetAccurateName()} object",
                OperatingValue.ElevatorGroup => $"{nameof(ElevatorGroup)} enum value",
                _ => flag.ToString()
            }).ToList();

        return string.Join(" OR ", values);
    }
    
    protected ResultStacker Rs => new($"Converting argument {Name} ({GetType().Name}) failed.");
    
    protected ArgumentEvaluation<T> CustomConvertSolution<T>(
        BaseToken token, 
        Func<string, ArgumentEvaluation<T>.EvalRes> convertMethod)
    {
        return VariableParser.IsValueSyntaxUsedInString(token.GetValue(), Script, out var replacedVariablesFunc)
            ? new(() => convertMethod(replacedVariablesFunc()))
            : new(convertMethod(token.GetValue()));
    }
    
    protected ArgumentEvaluation<T> MultipleSolutionConvert<T>(
        BaseToken token, 
        Dictionary<OperatingValue, Func<object, ArgumentEvaluation<T>.EvalRes>> converters)
    {
        // ReSharper disable once InvertIf
        if (Input == 0) converters.Keys.ForEachItem(k => Input |= k);
        
        return VariableParser.IsValueSyntaxUsedInString(token.GetValue(), Script, out var replacedVariablesFunc)
            ? new(() => ConvertWithDefaultConverters(replacedVariablesFunc(), converters))
            : new(ConvertWithDefaultConverters(token.GetValue(), converters));
    }
    
    protected ArgumentEvaluation<T> SingleSolutionConvert<T>(
        BaseToken token, 
        OperatingValue operatingValue,
        Func<object, ArgumentEvaluation<T>.EvalRes>? convertMethod = null)
    {
        Input = operatingValue;
        Dictionary<OperatingValue, Func<object, ArgumentEvaluation<T>.EvalRes>> converter = new()
        {
            [operatingValue] = value =>
            {
                try
                {
                    return convertMethod is not null
                        ? convertMethod(value)
                        : (T)value;
                }
                catch (InvalidCastException)
                {
                    throw new AndrzejFuckedUpException($"Cannot cast {value} ({value.GetType().GetAccurateName()}) to {typeof(T).GetAccurateName()}");
                }
            }
        };
        
        return VariableParser.IsValueSyntaxUsedInString(token.GetValue(), Script, out var replacedVariablesFunc)
            ? new(() => ConvertWithDefaultConverters<T>(replacedVariablesFunc(), converter))
            : new(ConvertWithDefaultConverters(token.GetValue(), converter));
    }

    private class ValueHandlerInfo
    {
        public required string Value { get; init; }
        public required Script Script { get; init; }
    }
    
    private delegate TryGet<object?> ValueHandler(ValueHandlerInfo info);

    private static Dictionary<OperatingValue, ValueHandler> ValueHandlers => new()
    {
        [OperatingValue.Boolean] = GetBoolean,
        [OperatingValue.Color] = GetColor,
        [OperatingValue.DoorName] = GetEnum<DoorName>,
        [OperatingValue.RoomName] = GetEnum<RoomName>,
        [OperatingValue.FacilityZone] = GetEnum<FacilityZone>,
        [OperatingValue.ItemType] = GetEnum<ItemType>,
        [OperatingValue.ElevatorGroup] = GetEnum<ElevatorGroup>,
        [OperatingValue.Text] = GetText,
        [OperatingValue.Float] = GetFloat,
        [OperatingValue.Int] = GetInt,
        [OperatingValue.Duration] = GetDuration,
        [OperatingValue.Percentage] = GetPercentage,
        [OperatingValue.AllOfType] = GetAllOfType,
        [OperatingValue.DoorReference] = GetSingleDoorReference,
        [OperatingValue.RoomReference] = GetSingleRoomReference,
        [OperatingValue.ItemReference] = GetItemReference,
        //[OperatingValue.RoomReferences] = GetReferences<Room>,
        //[OperatingValue.ItemReferences] = GetReferences<Item>,
        //[OperatingValue.DoorReferences] = GetReferences<Door>
    };
    
    protected ArgumentEvaluation<T>.EvalRes ConvertWithDefaultConverters<T>(
        string value, 
        Dictionary<OperatingValue, Func<object, ArgumentEvaluation<T>.EvalRes>>? converters
    )
    {
        foreach (var flag in Enum
                     .GetValues(typeof(OperatingValue))
                     .Cast<OperatingValue>()
                     .Where(flag => flag is not OperatingValue.Custom)
                     .Where(flag => Input.HasFlag(flag)))
        {
            if (!ValueHandlers.TryGetValue(flag, out var handler))
            {
                throw new AndrzejFuckedUpException($"there is no handler for flag {flag}");
            }
            
            if (handler(new ValueHandlerInfo { Value = value, Script = Script })
                .HasErrored(out var handlerError, out var handlerResult))
            {
                //return Rs.Add(handlerError);
                continue;
            }
            
            TryGet<object?> convert;
            if (converters is not null && converters.ContainsKey(flag))
            {
                convert = converters[flag](handlerResult!);
            }
            else
            {
                throw new AndrzejFuckedUpException($"Argument {Name} doesn't have converter for {flag} value.");
            }

            if (convert.HasErrored(out var convertError, out var result))
            {
                //return Rs.Add(convertError);
            }

            if (result is ArgumentEvaluation<T>.EvalRes evalRes)
            {
                return evalRes;
            }
            
            throw new AndrzejFuckedUpException(
                $"Result {result?.GetType().GetAccurateName() ?? "null"} for argument {Name} cannot be converted to " +
                $"{typeof(T).GetAccurateName()}.");
        }
        
        return Rs.Add($"Input '{value}' is not a {GetExpectedValues()}");
    }
    
    private static TryGet<object?> GetReference<T>(ValueHandlerInfo info)
    {
        var objResult = ObjectReferenceSystem.TryRetreiveObject(info.Value);
        if (objResult.HasErrored(out var error, out var obj))
        {
            return error;
        }
        
        if (obj is not T castedObj)
        {
            return $"Value '{info.Value}' is not a valid {typeof(T).GetAccurateName()} reference.";
        }
        
        return castedObj;
    }

    private static TryGet<object?> GetSingleDoorReference(ValueHandlerInfo info) => GetReference<Door>(info);

    private static TryGet<object?> GetSingleRoomReference(ValueHandlerInfo info) => GetReference<Room>(info);

    private static TryGet<object?> GetItemReference(ValueHandlerInfo info) => GetReference<Item>(info);

    private static TryGet<object?> GetText(ValueHandlerInfo info) => info.Value;

    private static TryGet<object?> GetFloat(ValueHandlerInfo info)
    {
        if (!float.TryParse(info.Value, out var result))
        {
            return $"Value '{info.Value}' cannot be interpreted as a number.";
        }
        
        return result;
    }

    private static TryGet<object?> GetInt(ValueHandlerInfo info)
    {
        if (!int.TryParse(info.Value, out var result))
        {
            return $"Value '{info.Value}' cannot be interpreted as an integer (whole number) value.";
        }
        return result;
    }

    private static TryGet<object?> GetDuration(ValueHandlerInfo info)
    {
        var value = info.Value;

        if (TimeSpan.TryParse(value, out var result) && result.TotalMilliseconds > 0)
        {
            return result;
        }

        var unitIndex = Array.FindIndex(value.ToCharArray(), char.IsLetter);
        if (unitIndex == -1)
        {
            return "No unit provided.";
        }

        var valuePart = value.Take(unitIndex).ToArray();
        if (!double.TryParse(string.Join("", valuePart), out var valueAsDouble))
        {
            return $"Value part '{string.Join("", valuePart)}' is not a valid number.";
        }
        
        if (valueAsDouble < 0)
        {
            return "Duration cannot be negative.";
        }

        var unit = value.Substring(unitIndex);
        TimeSpan? timeSpan = unit switch
        {
            "s" => TimeSpan.FromSeconds(valueAsDouble),
            "ms" => TimeSpan.FromMilliseconds(valueAsDouble),
            "m" => TimeSpan.FromMinutes(valueAsDouble),
            "h" => TimeSpan.FromHours(valueAsDouble),
            "d" => TimeSpan.FromDays(valueAsDouble),
            _ => null
        };

        if (timeSpan is null)
        {
            return $"Provided unit {unit} is not valid.";
        }

        return timeSpan.Value;
    }

    private static TryGet<object?> GetEnum<TEnum>(ValueHandlerInfo info) where TEnum : struct, Enum
    {
        if (!Enum.TryParse<TEnum>(info.Value, true, out var result))
        {
            return $"Value '{info.Value}' is not a valid {typeof(TEnum).Name} enum value.";
        }
        return result;
    }

    private static TryGet<object?> GetBoolean(ValueHandlerInfo info)
    {
        if (ExpressionSystem.EvalCondition(info.Value, info.Script).WasSuccessful(out var condResult))
        {
            return condResult;
        }
        
        if (bool.TryParse(info.Value, out var parseResult))
        {
            return parseResult;
        }

        return $"Value '{info.Value}' cannot be interpreted as a boolean value or condition.";
    }

    private static TryGet<object?> GetColor(ValueHandlerInfo info)
    {
        var value = info.Value;
        
        if (string.IsNullOrEmpty(value) || value[0] != '#')
        {
            return "Color must start with # character.";
        }

        value = value.Substring(1);

        switch (value.Length)
        {
            // RRGGBB
            case 6 when uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out uint hexVal):
            {
                float r = ((hexVal & 0xFF0000) >> 16) / 255f;
                float g = ((hexVal & 0x00FF00) >> 8) / 255f;
                float b = (hexVal & 0x0000FF) / 255f;
                return new Color(r, g, b, 1f);
            }
        
            // RRGGBBAA
            case 8 when uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out uint hexVal):
            {
                float r = ((hexVal & 0xFF000000) >> 24) / 255f;
                float g = ((hexVal & 0x00FF0000) >> 16) / 255f;
                float b = ((hexVal & 0x0000FF00) >> 8) / 255f;
                float a = (hexVal & 0x000000FF) / 255f;
                return new Color(r, g, b, a);
            }
            default:
                return $"Invalid color format. Expected 6 (RRGGBB) or 8 (RRGGBBAA) hex characters, got {value.Length}.";
        }
    }

    private static TryGet<object?> GetAllOfType(ValueHandlerInfo info)
    {
        return info.Value != "*" 
            ? $"AllOfType accepts only '*' character, got '{info.Value}'." 
            : null!;
    }
        
    private static TryGet<object?> GetReferences<T>(ValueHandlerInfo info)
    {
        var objResult = ObjectReferenceSystem.TryRetreiveObject(info.Value);
        if (objResult.HasErrored(out var error, out var obj))
        {
            return error;
        }

        return obj switch
        {
            T singleItem => new[] { singleItem },
            IEnumerable<T> collection => new(collection, null),
            _ => $"Value '{info.Value}' is not a valid {typeof(T).GetAccurateName()} or " +
                 $"{typeof(IEnumerable<T>).GetAccurateName()} reference."
        };
    }
    
    private static TryGet<object?> GetPercentage(ValueHandlerInfo info)
    {
        if (!info.Value.EndsWith("%"))
        {
            return "Percentage must end with % character.";
        }
        
        var valueWithoutPercentage = info.Value.TrimEnd('%');
        if (!float.TryParse(valueWithoutPercentage, out var percentage))
        {
            return $"Value '{valueWithoutPercentage}' cannot be interpreted as a number.";
        }
        
        if (percentage is < 0 or > 100)
        {
            return "Percentage must be between 0 and 100.";
        }
        
        return percentage / 100f;
    }
}