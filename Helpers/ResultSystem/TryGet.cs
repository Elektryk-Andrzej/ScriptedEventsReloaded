using System.Diagnostics.Contracts;
using SER.Helpers.Exceptions;

namespace SER.Helpers.ResultSystem;

public class TryGet<TValue>(TValue? value, string? errorMsg)
{
    [Pure] public virtual TValue? Value => value;
    [Pure] public virtual string? ErrorMsg => errorMsg;
    protected bool WasSuccess => string.IsNullOrEmpty(errorMsg);

    [Pure]
    public bool HasErrored(out string error)
    {
        error = ErrorMsg ?? "";
        return !WasSuccess;
    }

    [Pure]
    public bool HasErrored(out string error, out TValue val)
    {
        error = ErrorMsg ?? "";
        val = Value!;
        return !WasSuccess;
    }
    
    [Pure]
    public bool WasSuccessful()
    {
        return WasSuccess;
    }
    
    [Pure]
    public bool WasSuccessful(out TValue val)
    {
        val = Value!;
        return WasSuccess;
    }

    [Pure]
    public static implicit operator string(TryGet<TValue> result)
    {
        return result.ErrorMsg ?? throw new AndrzejFuckedUpException("implicit operator string(TryGet<TValue> result) called when not errored");
    }

    [Pure]
    public static implicit operator TryGet<TValue>(TValue value)
    {
        return new TryGet<TValue>(value, string.Empty);
    }

    [Pure]
    public static implicit operator TryGet<TValue>(Result res)
    {
        if (res.HasErrored(out var msg)) return new TryGet<TValue>(default, msg);

        throw new AndrzejFuckedUpException("implicit operator TryGet<TValue>(Result res) called when not errored");
    }

    [Pure]
    public static implicit operator TryGet<TValue>(string msg)
    {
        return new TryGet<TValue>(default, msg);
    }
    
    [Pure]
    public static TryGet<TValue> Error(string errorMsg)
    {
        return new TryGet<TValue>(default, errorMsg);
    }
    
    [Pure]
    public static TryGet<TValue> Success(TValue value)
    {
        return new TryGet<TValue>(value, null);
    }
}