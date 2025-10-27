using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
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
        if (token is NumberToken numberToken)
        {
            return VerifyRange(numberToken.Value);
        }
        
        if (token is not IValueCapableToken<LiteralValue>)
        {
            return $"Value '{token.RawRep}' cannot represent a number.";
        }
        
        if (TryParse(token).WasSuccessful(out var finalValue))
        {
            return finalValue;
        }

        return new(() => TryParse(token));
    }

    private TryGet<int> TryParse(BaseToken token)
    {
        if (token.TryGetLiteralValue<NumberValue>().HasErrored(out var error, out var value))
        {
            return new Result(false, error) + $"Value '{token.RawRep}' does not represent a valid number.";
        }
        
        return VerifyRange(value);
    }
    
    private TryGet<int> VerifyRange(NumberValue input)
    {
        var result = (int)input.ExactValue;
                
        if (result < _minValue)
            return $"Value '{result}' is lower than allowed minimum value {_minValue}.";
                
        if (result > _maxValue)
            return $"Value '{result}' is higher than allowed maximum value {_maxValue}.";

        return result;
    }
}