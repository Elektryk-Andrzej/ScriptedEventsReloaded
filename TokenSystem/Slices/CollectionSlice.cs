using System.Collections.Generic;
using System.Linq;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Structures;
using StringBuilder = System.Text.StringBuilder;

namespace SER.TokenSystem.Slices;

public class CollectionSlice : Slice
{
    private uint _depth = 1;
    private bool _ignoreNext = false;
    private readonly CollectionSliceInfo _info;
    private readonly StringBuilder _value = new();

    public CollectionBrackets Type => _info.Type;
    
    public override string Value => _value.ToString();

    public record CollectionSliceInfo(char Start, char End, CollectionBrackets Type);

    public static readonly CollectionSliceInfo[] CollectionSliceInfos =
    [
        new('{', '}', CollectionBrackets.Curly),
        new('(', ')', CollectionBrackets.Round),
        new('"', '"', CollectionBrackets.Quotes),
    ];

    public static readonly HashSet<char> CollectionStarters = CollectionSliceInfos.Select(i => i.Start).ToHashSet();

    public CollectionSlice(char startChar) : base(startChar)
    {
        if (!CollectionStarters.Contains(startChar)) 
            throw new AndrzejFuckedUpException();
        
        _info = CollectionSliceInfos.FirstOrDefault(i => i.Start == startChar) 
                ?? throw new AndrzejFuckedUpException();
    }

    public override bool CanContinueAfterAdd(char c)
    {
        PrivateRawRepresentation.Append(c);
        if (_ignoreNext)    
        {
            _ignoreNext = false;
            _value.Append(c);
            return true;
        }
        
        if (c == '`')
        {
            _ignoreNext = true;
            return true;
        }

        char[] startChars = [_info.Start];
        char[] endChars = [_info.End];
        if (_info.Type == CollectionBrackets.Quotes)
        {
            var curly = CollectionSliceInfos.First(i => i.Type == CollectionBrackets.Curly);
            startChars = startChars.Append(curly.Start).ToArray();
            endChars = endChars.Append(curly.End).ToArray();
        }
        
        if (endChars.Contains(c) && --_depth <= 0)
        {
            return false;
        }
        
        _value.Append(c);

        if (startChars.Contains(c))
        {
            _depth++;
        }
        
        return true;
    }

    public override Result VerifyState()
    {
        return Result.Assert(_depth == 0, $"Collection '{RawRep}' was not closed.");
    }
}