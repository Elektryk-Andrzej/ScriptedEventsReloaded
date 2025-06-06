﻿using System;
using System.Text;

namespace SER.Helpers.Extensions;

public static class TypeExtensions
{
    // done by chatgpt
    public static string GetAccurateName(this Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        var sb = new StringBuilder();
        string name = type.Name;
        int index = name.IndexOf('`');
        if (index > 0)
            name = name.Substring(0, index);

        sb.Append(name);
        sb.Append('<');
        var args = type.GetGenericArguments();
        for (int i = 0; i < args.Length; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(args[i].GetAccurateName()); // recursion for nested generics
        }
        sb.Append('>');
        return sb.ToString();
    }
}