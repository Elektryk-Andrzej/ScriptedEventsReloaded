using System;
using SER.Helpers.Exceptions;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;


public class FloatArgument : BaseMethodArgument
{
    private readonly float? _minValue;
    private readonly float? _maxValue;
    
    public FloatArgument(string name, float? minValue = null, float? maxValue = null) : base(name)
    {
        if (minValue.HasValue && maxValue.HasValue && minValue.Value > maxValue.Value)
        {
            throw new DeveloperFuckupException(
                $"{nameof(FloatArgument)} has minValue at {minValue.Value} and maxValue at {maxValue.Value}.");
        }
        
        _minValue = minValue;
        _maxValue = maxValue;
    }
    
    public override OperatingValue Input => OperatingValue.Float;

    public override string? AdditionalDescription
    {
        get
        {
            if (_minValue.HasValue && _maxValue.HasValue)
            {
                return $"Value must be at least {_minValue} and most {_maxValue} e.g. " +
                       $"{Math.Round(UnityEngine.Random.Range(_minValue.Value, _maxValue.Value), 2)}";
            }

            if (_minValue.HasValue)
            {
                return $"Value must be at least {_minValue} e.g. {_minValue + 2f}";
            }

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (_maxValue.HasValue)
            {
                return $"Value must be at most {_maxValue} e.g. {_maxValue - 2f}";
            }

            return null;
        }
    }

    public ArgumentEvaluation<float> GetConvertSolution(BaseToken token)
    {
        return DefaultConvertSolution<float>(token, new()
        {
            [OperatingValue.Float] = o =>
            {
                var result = (float)o;
                
                if (result < _minValue)
                    return Rs.Add($"Value '{result}' is lower than allowed minimum value {_minValue}.");
                
                if (result > _maxValue)
                    return Rs.Add($"Value '{result}' is higher than allowed maximum value {_maxValue}.");

                return result;
            }
        });
    }
}