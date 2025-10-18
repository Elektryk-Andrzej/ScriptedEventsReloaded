using System.Linq;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ScriptSystem.Structures;
using SER.TokenSystem.Slices;

namespace SER.TokenSystem.Tokens;

public class ParenthesesToken : BaseToken
{
    private BaseToken[]? _tokens = null;
    public TryGet<BaseToken[]> TryGetTokens()
    {
        if (_tokens is not null)
        {
            return _tokens;
        }

        if (Slice is null)
        {
            throw new AndrzejFuckedUpException();
        }

        Result error = $"Failed to get underlying tokens in the '{Slice.RawRepresentation}' parentheses.";
        if (Tokenizer.TokenizeLine(Slice.Value, Script, LineNum)
            .HasErrored(out var tokenizeError, out var tokens))
        {
            return error + tokenizeError;
        }

        return _tokens = tokens.ToArray();
    }

    protected override Result InternalParse(Script scr)
    {
        if (Slice is CollectionSlice { SliceType: CollectionSliceType.Round })
        {
            return true;
        }

        
        return $"Slice '{Slice.RawRepresentation}' is not in round brackets.";
    }

    public TryGet<object> ParseExpression()
    {
        if (TryGetTokens().HasErrored(out var error, out var tokens))
        {
            return error;
        }
        
        return ExpressionReslover.ParseExpression(tokens, Script);
    }
}