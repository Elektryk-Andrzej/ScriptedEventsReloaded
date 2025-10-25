using System;
using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;
using Random = UnityEngine.Random;

namespace SER.ArgumentSystem.Arguments;

public class FloatArgument : Argument
{
    private readonly float? _minValue;
    private readonly float? _maxValue;

    public FloatArgument(string name, float? minValue = null, float? maxValue = null) : base(name)
    {
        if (minValue.HasValue && maxValue.HasValue && minValue.Value > maxValue.Value)
        {
            throw new AndrzejFuckedUpException(
                $"{nameof(FloatArgument)} has minValue at {minValue.Value} and maxValue at {maxValue.Value}.");
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
                return $"A number which is at least {_minValue} and most {_maxValue} e.g. " +
                       $"{Math.Round(Random.Range(_minValue.Value, _maxValue.Value), 2)}";
            }

            if (_minValue.HasValue)
            {
                return $"A number which is at least {_minValue} e.g. {_minValue + 2f}";
            }

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (_maxValue.HasValue)
            {
                return $"A number which is at most {_maxValue} e.g. {_maxValue - 2f}";
            }

            return "Any number e.g. 2.5";
        }
    }

    [UsedImplicitly]
    public DynamicTryGet<float> GetConvertSolution(BaseToken token)
    {
        if (token is NumberToken number)
        {
            return VerifyRange((float)number.Value.Value);
        }
        
        if (token is not IValueCapableToken<LiteralValue>)
        {
            return $"Value '{token.RawRep}' cannot represent a number.";
        }
        
        if (TryParse(token).WasSuccessful(out var value))
        {
            return value;
        }

        return new(() => TryParse(token));
    }

    private TryGet<float> TryParse(BaseToken token)
    {
        if (token.TryGetLiteralValue<NumberValue>().HasErrored(out var error, out var value))
        {
            return error;
        }

        return VerifyRange((float)value.Value);
    }

    private TryGet<float> VerifyRange(float value)
    {
        if (value < _minValue)
            return $"Value {value} is lower than allowed minimum value {_minValue}.";
            
        if (value > _maxValue)
            return $"Value {value} is higher than allowed maximum value {_maxValue}.";

        return value;
    }
}