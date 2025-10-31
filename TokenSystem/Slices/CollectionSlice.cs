﻿using System.Collections.Generic;
using System.Linq;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Structures;
using StringBuilder = System.Text.StringBuilder;

namespace SER.TokenSystem.Slices;

public class CollectionSlice : Slice
{
    private bool _ignoreNext = false;
    private readonly Stack<CollectionSliceInfo> _contexts = new();
    private readonly StringBuilder _value = new();
    private readonly CollectionSliceInfo _rootInfo;

    public CollectionBrackets Type => _rootInfo.Type;
    public override string Value => _value.ToString();

    public record CollectionSliceInfo(char Start, char End, CollectionBrackets Type);

    public static readonly CollectionSliceInfo[] CollectionSliceInfos =
    [
        new('{', '}', CollectionBrackets.Curly),
        new('(', ')', CollectionBrackets.Round),
        new('"', '"', CollectionBrackets.Quotes),
    ];

    public static readonly HashSet<char> CollectionStarters = CollectionSliceInfos
        .Select(i => i.Start)
        .ToHashSet();

    public CollectionSlice(char startChar) : base(startChar)
    {
        if (!CollectionStarters.Contains(startChar))
            throw new AndrzejFuckedUpException();

        _rootInfo = CollectionSliceInfos.First(i => i.Start == startChar);
        _contexts.Push(_rootInfo);
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
        
        var current = _contexts.Peek();
        
        if (c == current.End)
        {
            _contexts.Pop();
            
            if (_contexts.Count == 0)
            {
                return false;
            }

            _value.Append(c);
            return true;
        }
        
        var opener = CollectionSliceInfos.FirstOrDefault(i => i.Start == c);
        if (opener != null)
        {
            if (current.Type == CollectionBrackets.Quotes)
            {
                if (opener.Type == CollectionBrackets.Curly)
                    _contexts.Push(opener);
            }
            else
            {
                _contexts.Push(opener);
            }
        }

        _value.Append(c);
        return true;
    }

    public override Result VerifyState()
    {
        return Result.Assert(_contexts.Count == 0, $"Collection '{RawRep}' was not closed.");
    }
}
