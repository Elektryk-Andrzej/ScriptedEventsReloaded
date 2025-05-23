using System;
using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.MethodSystem.Exceptions;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.VariableSystem;
using UnityEngine;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public abstract class BaseMethodArgument(string name)
{
    public string Name { get; } = name;
    
    public bool ConsumesRemainingArguments { get; init; } = false;
    
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

    public abstract OperatingValue Input { get; }
    
    public abstract string? AdditionalDescription { get; }

    public Script Script { get; set; } = null!;
    
    public string GetExpectedValues()
    {
        var values = (
            from flag in Enum.GetValues(typeof(OperatingValue)).Cast<OperatingValue>()
            where Input.HasFlag(flag)
            select flag switch
            {
                OperatingValue.DoorName => $"{nameof(DoorName)} enum value",
                OperatingValue.RoomName => $"{nameof(RoomName)} enum value",
                OperatingValue.FacilityZone => $"{nameof(FacilityZone)} enum value",
                OperatingValue.DoorReferences => $"reference to {typeof(IEnumerable<Door>).GetAccurateName()} object",
                OperatingValue.SingleDoorReference => $"reference to a single {typeof(Door).GetAccurateName()} object",
                OperatingValue.RoomReferences => $"reference to {typeof(IEnumerable<Room>).GetAccurateName()} object",
                OperatingValue.SingleRoomReference => $"reference to a single {typeof(Room).GetAccurateName()} object",
                OperatingValue.AllOfType => "* character (gets all)",
                OperatingValue.Boolean => "true/false value or a condition",
                OperatingValue.Color => "color e.g. #F2137F",
                OperatingValue.Text => "any text e.g. (Never Gonna Give You Up)",
                OperatingValue.Duration => "duration e.g. 5.3s",
                OperatingValue.CustomEnum => "custom enum value (check additional description)",
                OperatingValue.Float => "rational number e.g. 6.9",
                OperatingValue.Int => "integer number e.g. 420",
                OperatingValue.CustomOption => "valid option (check additional description)",
                OperatingValue.Players => "player variable e.g @myPlayers",
                OperatingValue.Player => "player variable with a single player e.g. @evPlayer",
                OperatingValue.Percentage => "percentage e.g. 37%",
                OperatingValue.CustomReference => "custom reference (check additional description)",
                OperatingValue.Script => "script",
                OperatingValue.Variable => "any variable",
                _ => throw new DeveloperFuckupException()
            }).ToList();

        return string.Join(" OR ", values);
    }
    
    protected ResultStacker Rs => new($"Converting argument {Name} ({GetType().Name}) failed.");

    protected ArgumentEvaluation<T> CustomConvertSolutionWithJoker<T>(
        BaseToken token, 
        Func<string, ArgumentEvaluation<T>.EvalRes> convertMethod,
        ArgumentEvaluation<T> valueOnStar)
    {
        return token.GetValue() is "*" 
            ? valueOnStar 
            : CustomConvertSolution(token, convertMethod);
    }
    
    protected ArgumentEvaluation<T> CustomConvertSolution<T>(
        BaseToken token, 
        Func<string, ArgumentEvaluation<T>.EvalRes> convertMethod)
    {
        return VariableParser.IsVariableUsedInString(token.GetValue(), Script, out var replacedVariablesFunc)
            ? new(() => convertMethod(replacedVariablesFunc()))
            : new(convertMethod(token.GetValue()));
    }
    
    protected ArgumentEvaluation<T> DefaultConvertSolution<T>(
        BaseToken token, 
        Dictionary<OperatingValue, Func<object, ArgumentEvaluation<T>.EvalRes>>? converters)
    {
        return VariableParser.IsVariableUsedInString(token.GetValue(), Script, out var replacedVariablesFunc)
            ? new(() => ConvertWithDefaultConverters(replacedVariablesFunc(), converters))
            : new(ConvertWithDefaultConverters(token.GetValue(), converters));
    }

    protected ArgumentEvaluation<T>.EvalRes ConvertWithDefaultConverters<T>(
        string value, 
        Dictionary<OperatingValue, Func<object, ArgumentEvaluation<T>.EvalRes>>? converters
    )
    {
        List<(OperatingValue, string)> errors = [];
        foreach (var flag in Enum.GetValues(typeof(OperatingValue)).Cast<OperatingValue>())
        {
            if (!Input.HasFlag(flag)) continue;

            object result;
            switch (flag)
            {
                case OperatingValue.Boolean:
                {
                    if (GetBoolean(value).HasErrored(out var error, out var res))
                    {
                        errors.Add((flag, error));
                        continue;
                    }
                    
                    result = res;
                    break;
                }
                case OperatingValue.Color:
                {
                    if (GetColor(value).HasErrored(out var error, out var res))
                    {
                        errors.Add((flag, error));
                        continue;
                    }
                    
                    result = res;
                    break;
                }
                case OperatingValue.DoorName:
                {
                    if (GetEnum<DoorName>(value).HasErrored(out var error, out var res))
                    {
                        errors.Add((flag, error));
                        continue;
                    }
                    
                    result = res;
                    break;
                }
                case OperatingValue.RoomName:
                {
                    if (GetEnum<RoomName>(value).HasErrored(out var error, out var res))
                    {
                        errors.Add((flag, error));
                        continue;
                    }
                    
                    result = res;
                    break;
                }
                case OperatingValue.FacilityZone:
                {
                    if (GetEnum<FacilityZone>(value).HasErrored(out var error, out var res))
                    {
                        errors.Add((flag, error));
                        continue;
                    }
                    
                    result = res;
                    break;
                }
                case OperatingValue.DoorReferences:
                {
                    if (GetReference<Door>(value).WasSuccessful(out var res))
                    {
                        result = new[] { res };
                        break;
                    }
                    
                    if (GetReference<IEnumerable<Door>>(value).HasErrored(out var error, out var listRes))
                    {
                        errors.Add((flag, error));
                        continue;
                    }
                    
                    result = listRes;
                    break;
                }
                case OperatingValue.RoomReferences:
                {
                    if (GetReference<Room>(value).WasSuccessful(out var res))
                    {
                        result = new[] { res };
                        break;
                    }
                    
                    if (GetReference<IEnumerable<Room>>(value).HasErrored(out var error, out var listRes))
                    {
                        errors.Add((flag, error));
                        continue;
                    }
                    
                    result = listRes;
                    break;
                }
                case OperatingValue.AllOfType when value == "*":
                {
                    result = null!;
                    break;
                }
                case OperatingValue.Text:
                {
                    result = value;
                    break;
                }
                case OperatingValue.Duration:
                {
                    if (GetDuration(value).HasErrored(out var error, out var res))
                    {
                        errors.Add((flag, error));
                        continue;
                    }
                    
                    result = res;
                    break;
                }
                case OperatingValue.Float:
                {
                    if (GetFloat(value).HasErrored(out var error, out var res))
                    {
                        errors.Add((flag, error));
                        continue;
                    }
                    
                    result = res;
                    break;
                }
                default:
                {
                    return $"Input {value} failed to meet any of the following requirements: {GetExpectedValues()}.";
                }
            }

            if (result is T castedResult)
            {
                return castedResult;
            }
            
            if (converters is null || !converters.TryGetValue(flag, out var converter))
            {
                throw new DeveloperFuckupException($"Argument {Name} doesn't have converter for {flag} value.");
            }
            
            return converter(result);
        }

        throw new DeveloperFuckupException();
    }

    private TryGet<bool> GetBoolean(string value)
    {
        if (Condition.TryEval(value, Script).WasSuccessful(out var condResult))
        {
            return condResult;
        }
        
        if (bool.TryParse(value, out var parseResult))
        {
            return parseResult;
        }
        
        return Rs.Add($"Value '{value}' is not a valid true/false (boolean) value nor a condition leading to a boolean value.");
    }

    private TryGet<Color> GetColor(string value)
    {
        if (ColorUtility.TryParseHtmlString(value, out var result))
        {
            return result;
        }
        
        return Rs.Add($"Value '{value}' is not a valid color.");
    }

    private TryGet<TEnum> GetEnum<TEnum>(string value) where TEnum : struct, Enum
    {
        if (Enum.TryParse(value, true, out TEnum result))
        {
            return result;
        }
        
        return Rs.Add($"Value '{value}' is not a valid {typeof(TEnum).Name} enum value.");
    }
    
    private TryGet<T> GetReference<T>(string value)
    {
        if (!ObjectReferenceSystem.TryRetreiveObject(value).WasSuccessful(out var obj) || obj is not T castedObj)
        {
            return Rs.Add($"Value '{value}' is not a valid {typeof(T).GetAccurateName()} reference.");
        }

        return castedObj;
    }

    private TryGet<float> GetFloat(string value)
    {
        if (!float.TryParse(value, out var result))
            return Rs.Add($"Value '{value}' cannot be interpreted as a number.");
        
        return result;
    }

    private TryGet<int> GetInt(string value)
    {
        if (!int.TryParse(value, out var result))
            return Rs.Add($"Value '{value}' cannot be interpreted as an integer (whole number) value.");
        
        return result;
    }

    private TryGet<TimeSpan> GetDuration(string value)
    {
        if (TimeSpan.TryParse(value, out var result) && result.TotalMilliseconds > 0)
        {
            return result;
        }
        
        var unitIndex = Array.FindIndex(value.ToCharArray(), char.IsLetter);
        if (unitIndex == -1)
        {
            return Rs.Add("No unit provided.");
        }

        var valuePart = value.Take(unitIndex).ToArray();
        if (!double.TryParse(string.Join("", valuePart), out var valueAsDouble))
        {
            return Rs.Add($"Value part '{string.Join("", valuePart)}' is not a valid number.");
        }
        if (valueAsDouble < 0)
        {
            return Rs.Add("Duration cannot be negative..");
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
            return Rs.Add($"Provided unit {unit} is not valid.");

        return timeSpan.Value;
    }
}