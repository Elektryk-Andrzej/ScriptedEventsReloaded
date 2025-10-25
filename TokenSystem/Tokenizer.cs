using System;
using System.Collections.Generic;
using System.Linq;
using SER.Helpers;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Slices;
using SER.TokenSystem.Structures;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Variables;

namespace SER.TokenSystem;

public static class Tokenizer
{
    public static readonly Type[] OrderedImportanceTokensFromSingleSlices =
    [
        typeof(KeywordToken),
        typeof(BoolToken),
        typeof(MethodToken),
        typeof(FlagToken),
        typeof(FlagArgumentToken),
        typeof(CommentToken),
        typeof(SymbolToken),
        typeof(NumberToken),
        typeof(PlayerVariableToken),
        typeof(LiteralVariableToken),
        typeof(CollectionVariableToken),
        typeof(ReferenceVariableToken),
        typeof(DurationToken)
    ];
    
    public static readonly Type[] OrderedImportanceTokensFromCollectionSlices =
    [
        typeof(ExpressionToken),
        typeof(ParenthesesToken),
        typeof(TextToken),
    ];
    
    public static TryGet<Line[]> GetInfoFromMultipleLines(string content)
    {
        List<Line> outList = [];
        
        var lines = content.Split(["\r\n", "\n", "\r"], StringSplitOptions.None);
        for (uint index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            var info = new Line
            {
                RawRepresentation = line,
                LineNumber = index + 1
            };
            
            outList.Add(info);
        }

        return outList.ToArray();
    }

    public static Result SliceLine(Line line)
    {
        if (SliceLine(line.RawRepresentation).HasErrored(out var err, out var slices))
        {
            return err;
        }
        
        line.Slices = slices.ToArray();
        return true;
    }
    
    /// <summary>
    /// Decides the line if it's collection slice or single slice.
    /// </summary>
    /// <param name="line">Line to parse</param>
    /// <returns>The slices for the specificalious line</returns>
    public static TryGet<IEnumerable<Slice>> SliceLine(string line)
    {
        List<Slice> outList = [];
        Slice? currentSlice = null;
        foreach (char currentChar in line)
        {
            if (currentSlice is null)
            {
                if (char.IsWhiteSpace(currentChar))
                {
                    continue;
                }

                currentSlice = CollectionSlice.CollectionStarters.Contains(currentChar) 
                    ? new CollectionSlice(currentChar) 
                    : new SingleSlice(currentChar);
                
                continue;
            }

            if (currentSlice.CanContinueAfterAdd(currentChar))
            {
                continue;
            }

            if (currentSlice.VerifyState().HasErrored(out var error))
            {
                return error;
            }
            
            outList.Add(currentSlice);
            currentSlice = null;
        }

        if (currentSlice is not null)
        {
            if (currentSlice.VerifyState().HasErrored(out var error))
            {
                return error;
            }
            
            outList.Add(currentSlice);
        }

        return outList;
    }

    public static void TokenizeLine(Line line, Script scr)
    {
        line.Tokens = TokenizeLine(line.Slices, scr, line.LineNumber).ToArray();
    }

    public static TryGet<IEnumerable<BaseToken>> TokenizeLine(string line, Script scr, uint? lineNum)
    {
        if (SliceLine(line)
            .HasErrored(out var sliceError, out var slices))
        {
            return sliceError;
        }
        
        return TokenizeLine(slices, scr, lineNum).ToArray();
    }

    public static IEnumerable<BaseToken> TokenizeLine(IEnumerable<Slice> slices, Script scr, uint? lineNum)
    {
        var sliceArray = slices.ToArray();
        var tokens = sliceArray.Select(slice => GetTokenFromSlice(slice, scr, lineNum)).ToArray();
        Log.Debug(
            $"Slices [{sliceArray.Select(s => $"'{s.RawRepresentation}'").JoinStrings(" ")}] " +
            $"-> " +
            $"Tokens [{tokens.Select(t => $"<'{t.RawRep}' as {t.GetType().Name}>").JoinStrings(" ")}]");
        return tokens;
    }

    public static BaseToken GetTokenFromSlice(Slice slice, Script scr, uint? lineNum)
    {
        var tokenCollection = slice is CollectionSlice 
            ? OrderedImportanceTokensFromCollectionSlices 
            : OrderedImportanceTokensFromSingleSlices;
        
        foreach (var tokenType in tokenCollection)
        {
            var token = tokenType.CreateInstance<BaseToken>();
            if (token.TryInit(slice, scr, lineNum).WasSuccess)
            {
                return token;
            }
        }

        var unspecified = new BaseToken();
        unspecified.TryInit(slice, scr, lineNum);
        return unspecified;
    }
}