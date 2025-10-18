using System;
using System.Linq;
using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.ArgumentSystem.Structures;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ArgumentSystem.Arguments;

public class OptionsArgument(string name, params Option[] options) : Argument(name)
{
    public readonly Option[] Options = options;

    public override string InputDescription =>
        "One of the following options:\n > " +
        string.Join("\n > ", Options.Select(o => string.IsNullOrEmpty(o.Description) 
            ? o.Value
            : $"{o.Value} ({o.Description})"));

    [UsedImplicitly]
    public DynamicTryGet<string> GetConvertSolution(BaseToken token)
    {
        return new(() => InternalConvert(token.GetBestTextRepresentation(Script)));
    }

    private TryGet<string> InternalConvert(string value)
    {
        var option = Options.FirstOrDefault(opt 
            => opt.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));
        
        if (option == null)
        {
            return TryGet<string>.Error(
                $"Value '{value}' does not match any of the following options: " +
                $"{string.Join(", ", Options.Select(o => o.Value))}");
        }

        return TryGet<string>.Success(option.Value);
    }
}