using System.Text.RegularExpressions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ScriptSystem.Structures;
using SER.TokenSystem.Slices;
using SER.TokenSystem.Structures;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class TextToken : ValueToken<TextValue>, ILiteralValueToken
{
    private static readonly Regex ExpressionRegex = new(@"\{.*?\}", RegexOptions.Compiled);
    
    public TryGet<string> TextRepresentation(Script _) => TryGet<string>.Success(Value);

    public string ParsedValue(Script script) => ParseValue(Value, script);

    public bool ContainsExpressions => ExpressionRegex.IsMatch(Value);

    public static string ParseValue(string text, Script script) => ExpressionRegex.Replace(text, match =>
    {
        Result mainErr = $"Value '{match.Value}' is not a valid literal expression.";
        // ReSharper disable once DuplicatedSequentialIfBodies
        if (LiteralExpressionToken
            .TryGet(match.Value, script)
            .HasErrored(out var error, out var token))
        {
            script.Executor.Warn(mainErr + error, script);
            return "<error>";
        }

        if (token.GetLiteralValue(script).HasErrored(out error, out var result))
        {
            script.Executor.Warn(mainErr + error, script);
            return "<error>";
        }
            
        return result.ToString();
    });

    protected override Result InternalParse(Script scr)
    {
        if (Slice is not CollectionSlice { SliceType: CollectionSliceType.Quotes })
        {
            return "Text must be in quotes.";
        }
        
        Value = Slice.Value;
        return true;
    }
}