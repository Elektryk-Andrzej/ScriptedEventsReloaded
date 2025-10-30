using System.Linq;
using System.Text.RegularExpressions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Slices;
using SER.TokenSystem.Structures;
using SER.TokenSystem.Tokens.ExpressionTokens;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class TextToken : LiteralValueToken<TextValue>
{
    private static readonly Regex ExpressionRegex = new(@"\{.*?\}", RegexOptions.Compiled);

    public string ParsedValue() => ContainsExpressions ? ParseValue(Value, Script) : Value;

    public bool ContainsExpressions => ExpressionRegex.IsMatch(Value);

    public static string ParseValue(string text, Script script) => ExpressionRegex.Replace(text, match =>
    {
        if (!Tokenizer.SliceLine(match.Value).WasSuccessful(out var slices))
        {
            return "<error>";
        }

        if (slices.FirstOrDefault() is not CollectionSlice { Type: CollectionBrackets.Curly } collection)
        {
            return "<error>";
        }
        
        if (!ExpressionToken.TryParse(collection, script).WasSuccessful(out var token))
        {
            return "<error>";
        }

        if (token is not IValueToken valueToken)
        {
            return "<error>";
        }

        if (!valueToken.CanReturn<LiteralValue>(out var get))
        {
            return "<error>";
        }

        if (!get().WasSuccessful(out var value))
        {
            return "<error>";
        }
            
        return value.StringRep;
    });

    protected override IParseResult InternalParse(Script scr)
    {
        if (Slice is not CollectionSlice { Type: CollectionBrackets.Quotes })
        {
            return new Ignore();
        }
        
        Value = Slice.Value;
        return new Success();
    }

    public DynamicTryGet<string> GetDynamicResolver()
    {
        if (ContainsExpressions) return new(() => TryGet<string>.Success(ParsedValue()));
        return DynamicTryGet.Success(Value.ExactValue);
    }
}