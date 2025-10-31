using System.Linq;
using System.Text.RegularExpressions;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Slices;
using SER.TokenSystem.Structures;
using SER.TokenSystem.Tokens.ExpressionTokens;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class TextToken : LiteralValueToken<TextValue>
{
    private static readonly Regex ExpressionRegex = new(@"~?\{.*?\}", RegexOptions.Compiled);

    public string ParsedValue() => ContainsExpressions ? ParseValue(Value, Script) : Value;

    public bool ContainsExpressions => ExpressionRegex.IsMatch(Value);

    public static string ParseValue(string text, Script script) => ExpressionRegex.Replace(text, match =>
    {
        if (match.Value.StartsWith("~")) return match.Value.Substring(1);
        
        if (Tokenizer.SliceLine(match.Value).HasErrored(out var error, out var slices))
        {
            Log.Warn(script, error);
            return "<error>";
        }

        if (slices.FirstOrDefault() is not CollectionSlice { Type: CollectionBrackets.Curly } collection)
        {
            throw new AndrzejFuckedUpException();
        }
        
        // ReSharper disable once DuplicatedSequentialIfBodies
        if (ExpressionToken.TryParse(collection, script).HasErrored(out error, out var token))
        {
            Log.Warn(script, error);
            return "<error>";
        }

        if (((BaseToken)token).TryGet<LiteralValue>().HasErrored(out error, out var value))
        {
            Log.Warn(script, error);
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