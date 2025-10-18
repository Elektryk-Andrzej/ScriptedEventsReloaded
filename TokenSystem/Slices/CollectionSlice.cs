using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem.Structures;
using StringBuilder = System.Text.StringBuilder;

namespace SER.TokenSystem.Slices;

public class CollectionSlice : Slice
{
    private readonly char _startChar;
    private char EndChar => CollectionSliceInfo[_startChar].Item1;
    private uint _depth = 1;
    private bool _ignoreNext = false;

    public static readonly ReadOnlyDictionary<char, (char, CollectionSliceType)> CollectionSliceInfo = new(new Dictionary<char, (char, CollectionSliceType)>
    {
        ['{'] = ('}', CollectionSliceType.Curly),
        ['('] = (')', CollectionSliceType.Round),
        ['\"'] = ('\"', CollectionSliceType.Quotes)
    });

    public static readonly HashSet<char> CollectionStarters = CollectionSliceInfo.Keys.ToHashSet();

    public CollectionSliceType SliceType => CollectionSliceInfo[_startChar].Item2;

    private readonly StringBuilder _value = new();

    public CollectionSlice(char startChar) : base(startChar)
    {
        if (!CollectionStarters.Contains(startChar)) throw new AndrzejFuckedUpException();
        _startChar = startChar;
    }

    public override string Value => _value.ToString();
    
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
        
        if (c == EndChar && --_depth <= 0)
        {
            return false;
        }
        
        _value.Append(c);

        if (c == _startChar)
        {
            _depth++;
        }
        
        return true;
    }

    public override Result VerifyState()
    {
        return Result.Assert(_depth == 0, $"Collection '{RawRepresentation}' was not closed.");
    }
}