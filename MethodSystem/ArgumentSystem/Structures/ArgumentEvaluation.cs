using System;
using System.Diagnostics.Contracts;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultStructure;
using SER.MethodSystem.ArgumentSystem.Interfaces;

namespace SER.MethodSystem.ArgumentSystem.Structures;

[Pure]
public class ArgumentEvaluation<T> : IArgEvalRes
{
    public ArgumentEvaluation(EvalRes staticResult)
    {
        IsStatic = true;
        GetValue = () => staticResult.Value;
        GetResult = () => staticResult.Result;
    }

    public static implicit operator ArgumentEvaluation<T>(EvalRes staticResult)
    {
        return new ArgumentEvaluation<T>(staticResult);
    }

    public ArgumentEvaluation(string errorMsg)
    {
        IsStatic = true;
        GetValue = () => default!;
        GetResult = () => errorMsg;
    }

    public static implicit operator ArgumentEvaluation<T>(string errorMsg)
    {
        return new ArgumentEvaluation<T>(errorMsg);
    }

    public ArgumentEvaluation(T value)
    {
        IsStatic = true;
        GetValue = () => value;
        GetResult = () => true;
    }

    public static implicit operator ArgumentEvaluation<T>(T value)
    {
        return new ArgumentEvaluation<T>(value);
    }

    public ArgumentEvaluation(Func<EvalRes> dynamicResult)
    {
        IsStatic = false;
        GetValue = () => dynamicResult().Value;
        GetResult = () => dynamicResult().Result;
    }

    public static implicit operator ArgumentEvaluation<T>(Func<EvalRes> dynamicResult)
    {
        return new ArgumentEvaluation<T>(dynamicResult);
    }
    
    public static implicit operator ArgumentEvaluation<T>(Result result)
    {
        if (result.WasSuccess)
        {
            throw new AndrzejFuckedUpException("used result on success");
        }
        
        return new ArgumentEvaluation<T>(result.ErrorMsg);
    }
    
    public static implicit operator ArgumentEvaluation<T>(TryGet<T> tryGet)
    {
        if (tryGet.HasErrored(out var error, out var value))
        {
            return error;
        }

        return value;
    }
    
    [Pure]
    public Func<T> GetValue { get; init; }

    [Pure]
    public bool IsStatic { get; init; }
    
    [Pure]
    public Func<Result> GetResult { get; init; }

    [Pure]
    public class EvalRes
    {
        public required T Value { get; init; }
        public required Result Result { get; init; }

        public static implicit operator EvalRes(string res)
        {
            return new()
            {
                Value = default!,
                Result = res
            };
        }
        
        public static implicit operator EvalRes(Result res)
        {
            return new()
            {
                Value = default!,
                Result = res
            };
        }
        
        public static implicit operator EvalRes(T value)
        {
            return new()
            {
                Value = value,
                Result = true
            };
        }
        
        public static implicit operator EvalRes(TryGet<T> tryGet)
        {
            if (tryGet.HasErrored(out var error, out var value))
            {
                return error;
            }

            return value;
        }
    }
}