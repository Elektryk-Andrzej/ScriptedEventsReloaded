using System.Text.RegularExpressions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Slices;
using SER.TokenSystem.Structures;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class TextToken : LiteralValueToken<TextValue>
{
    private static readonly Regex ExpressionRegex = new(@"\{.*?\}", RegexOptions.Compiled);
    
    public TryGet<string> TextRepresentation(Script _) => TryGet<string>.Success(Value);

    public string ParsedValue() => ContainsExpressions ? ParseValue(Value, Script) : Value;

    public bool ContainsExpressions => ExpressionRegex.IsMatch(Value);

    public static string ParseValue(string text, Script script) => ExpressionRegex.Replace(text, match =>
    {
        Result mainErr = $"Value '{match.Value}' is not a valid literal expression.";
        // ReSharper disable once DuplicatedSequentialIfBodies
        if (ExpressionToken
            .TryGet(match.Value, script)
            .HasErrored(out var error, out var token))
        {
            script.Executor.Warn(mainErr + error, script);
            return "<error>";
        }

        if (token is not IValueCapableToken<LiteralValue> literal)
        {
            return "<error>";
        }

        if (literal.ExactValue.HasErrored(out error, out var result))
        {
            script.Executor.Warn(mainErr + error, script);
            return "<error>";
        }
            
        return result.Value.ToString();
    });

    protected override Result InternalParse(Script scr)
    {
        if (Slice is not CollectionSlice { Type: CollectionSliceType.Quotes })
        {
            return "Text must be in quotes.";
        }
        
        Value = Slice.Value;
        return true;
    }

    public DynamicTryGet<TextValue> GetDynamicResolver()
    {
        if (ContainsExpressions) return new(() => ParsedValue());
        return Value;
    }
    
    public DynamicTryGet<LiteralValue> GetDynamicResolverLiteral()
    {
        if (ContainsExpressions) return new(() => ParsedValue());
        return Value;
    }
}