using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.ValueSystem;
using UnityEngine;

namespace SER.ArgumentSystem.Arguments;

public class IntArgument : Argument
{
    private readonly int? _minValue;
    private readonly int? _maxValue;
    
    public IntArgument(string name, int? minValue = null, int? maxValue = null) : base(name)
    {
        if (minValue.HasValue && maxValue.HasValue && minValue.Value > maxValue.Value)
        {
            throw new AndrzejFuckedUpException(
                $"{nameof(IntArgument)} has minValue at {minValue.Value} and maxValue at {maxValue.Value}.");
        }
        
        _minValue = minValue;
        _maxValue = maxValue;
    }

    public override string InputDescription 
    {
        get
        {
            if (_minValue.HasValue && _maxValue.HasValue)
            {
                return $"Value must be at least {_minValue} and most {_maxValue} e.g. " +
                       $"{Random.Range(_minValue.Value, _maxValue.Value + 1)}";
            }

            if (_minValue.HasValue)
            {
                return $"Value must be at least {_minValue} e.g. {_minValue + 420}";
            }

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (_maxValue.HasValue)
            {
                return $"Value must be at most {_maxValue} e.g. {_maxValue - 42}";
            }

            throw new AndrzejFuckedUpException();
        }
    }
    
    [UsedImplicitly]
    public DynamicTryGet<int> GetConvertSolution(BaseToken token)
    {
        if (TryParse(token).WasSuccessful(out var finalValue))
        {
            return finalValue;
        }

        return new(() => TryParse(token));
    }

    private TryGet<int> TryParse(BaseToken token)
    {
        if (token.TryGetValue<NumberValue>().WasSuccessful(out var value))
        {
            return VerifyValue(value);
        }

        return $"Value '{token.RawRepresentation}' does not represent a valid number.";
    }
    
    private TryGet<int> VerifyValue(NumberValue input)
    {
        var result = (int)input.Value;
                
        if (result < _minValue)
            return $"Value '{result}' is lower than allowed minimum value {_minValue}.";
                
        if (result > _maxValue)
            return $"Value '{result}' is higher than allowed maximum value {_maxValue}.";

        return result;
    }
}