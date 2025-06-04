using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SER.Helpers.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Converts the first character of the given string to lowercase while keeping the rest of the string unchanged.
    /// </summary>
    /// <param name="str">The input string to be modified.</param>
    /// <returns>A new string with the first character decapitalized.</returns>
    [Pure]
    public static string Decapitalize(this string str)
    {
        return str.Substring(0, 1).ToLower() + str.Substring(1);
    }

    // python ahh
    [Pure]
    public static string Join(this string separator, IEnumerable<string> values)
    {
        return string.Join(separator, values);
    }
}