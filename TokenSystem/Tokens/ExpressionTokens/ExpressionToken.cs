﻿using System;
using System.Linq;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Slices;
using SER.TokenSystem.Structures;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens.ExpressionTokens;

public abstract class ExpressionToken : BaseToken, IValueToken
{
    protected override IParseResult InternalParse(Script scr)
    {
        if (Slice is not CollectionSlice { Type: CollectionBrackets.Curly } collection)
        {
            return new Ignore();
        }

        if (Tokenizer.TokenizeLine(collection.Value, scr, null).HasErrored(out var error, out var tokensEnum))
        {
            return new Error(error);
        }
        
        var tokens = tokensEnum.ToArray();
        if (tokens.Len == 0)
        {
            return new Error($"Expression '{collection.Value}' is empty.");
        }

        return InternalParse(tokens);
    }

    public static TryGet<ExpressionToken> TryParse(CollectionSlice slice, Script script)
    {
        if (Tokenizer.GetTokenFromSlice(slice, script, null).HasErrored(out var error, out var val))
        {
            return error;
        }

        if (val is not ExpressionToken expToken)
        {
            return $"Token '{slice.RawRep}' is not an {typeof(ExpressionToken).FriendlyTypeName()}, but a {val.GetType().FriendlyTypeName()}";
        }
        
        return expToken;
    }

    protected abstract IParseResult InternalParse(BaseToken[] tokens);

    public abstract TryGet<Value> Value();

    public abstract Type[]? PossibleValueTypes { get; }
    
    public bool IsConstant => false;
}