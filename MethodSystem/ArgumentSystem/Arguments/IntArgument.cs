using JetBrains.Annotations;
using SER.Helpers.Exceptions;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;


public class IntArgument : GenericMethodArgument
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

    public override string? AdditionalDescription
    {
        get
        {
            if (_minValue.HasValue && _maxValue.HasValue)
            {
                return $"Value must be at least {_minValue} and most {_maxValue} e.g. " +
                       $"{UnityEngine.Random.Range(_minValue.Value, _maxValue.Value + 1)}";
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

            return null;
        }
    }
    
    [UsedImplicitly]
    public ArgumentEvaluation<int> GetConvertSolution(BaseToken token)
    {
        return SingleSolutionConvert<int>(token, OperatingValue.Int, o =>
        {
            var result = (int)o;
                
            if (result < _minValue)
                return Rs.Add($"Value '{result}' is lower than allowed minimum value {_minValue}.");
                
            if (result > _maxValue)
                return Rs.Add($"Value '{result}' is higher than allowed maximum value {_maxValue}.");

            return result;
        });
    }
}