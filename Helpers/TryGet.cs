using System.Diagnostics.Contracts;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultStructure;

namespace SER.Helpers;

public class TryGet<TValue>(TValue? value, string? errorMsg)
{
    private TValue? Value => value;
    private bool WasSuccess => string.IsNullOrEmpty(errorMsg);
    private string? ErrorMsg => errorMsg;

    [Pure]
    public bool HasErrored()
    {
        return !WasSuccess;
    }
    
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
        return result.ErrorMsg ?? "";
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

        throw new DeveloperFuckupException("implicit operator TryGet<TValue>(Result res) called when not errored");
    }

    [Pure]
    public static implicit operator TryGet<TValue>(string msg)
    {
        return new TryGet<TValue>(default, msg);
    }
    
    [Pure]
    public static implicit operator TryGet<TValue>(ResultStacker stacker)
    {
        return new TryGet<TValue>(default, stacker.InitMsg);
    }
}