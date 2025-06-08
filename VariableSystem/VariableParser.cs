using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultStructure;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem;
using SER.ScriptSystem.ContextSystem;
using SER.ScriptSystem.ContextSystem.Contexts;
using SER.ScriptSystem.TokenSystem;
using SER.VariableSystem.Structures;

namespace SER.VariableSystem;

/// <summary>
///     Replaces variables in contaminated strings.
/// </summary>
public static class VariableParser
{
    public static string ReplaceVariablesInContaminatedString(string input, Script scr)
    {
        var result = new StringBuilder();
        var i = 0;

        while (i < input.Length)
        {
            if (input[i] != '{')
            {
                result.Append(input[i]);
                i++;
                continue;
            }

            var start = i;
            var depth = 1;
            i++;

            while (i < input.Length && depth > 0)
            {
                switch (input[i])
                {
                    case '{':
                        depth++;
                        break;
                    case '}':
                        depth--;
                        break;
                }

                i++;
            }

            if (depth > 0)
            {
                result.Append(input.Substring(start));
                break;
            }

            var inner = input.Substring(start + 1, i - start - 2);
            Log.Debug($"Changing {{{inner}}} to its value");

            void AddUnparsed()
            {
                result.Append($"{{{inner}}}");
            }

            if (string.IsNullOrEmpty(inner))
            {
                AddUnparsed();
                continue;
            }

            // method parsing
            if (char.IsUpper(inner[0]))
            {
                var tokens = new Tokenizer(scr).GetTokensFromLine(inner.ToCharArray(), -1);

                // todo: better system
                if (new Contexter(scr).LinkAllTokens([tokens])
                    .HasErrored(out var linkError, out var contexts))
                {
                    Log.Debug(linkError);
                    AddUnparsed();
                    continue;
                }

                if (contexts.Count != 1)
                {
                    Log.Debug($"{{{inner}}} should be a single context, but fetched {contexts.Count}");
                    AddUnparsed();
                    continue;
                }

                if (contexts.First() is not MethodContext methodContext)
                {
                    Log.Debug($"{{{inner}}} should be method, but is a {contexts.First().GetType().Name}");
                    AddUnparsed();
                    continue;
                }

                if (methodContext.Method is not TextReturningMethod textMethod)
                {
                    Log.Debug($"{{{inner}}} method does not return a value!");
                    AddUnparsed();
                    continue;
                }

                textMethod.Execute();
                result.Append(textMethod.TextReturn);
                continue;
            }

            if (scr.TryGetLiteralVariable(inner).WasSuccessful(out var variable))
            {
                Log.Debug($"success! {inner} is a valid var, setting {variable.Value()} as");
                result.Append(variable.Value());
                continue;
            }

            Log.Debug($"error! {inner} is not a valid variable");
            result.Append($"{{{inner}}}");
        }

        return result.ToString();
    }
    
    public static List<VariableCoordinate> GetVariableCoordinatesInContaminatedString(
        string input,
        Script scr
    )
    {
        var coordinates = new List<VariableCoordinate>();
        var i = 0;

        while (i < input.Length)
        {
            if (input[i] != '{')
            {
                i++;
                continue;
            }

            var start = i;
            var depth = 1;
            i++;

            while (i < input.Length && depth > 0)
            {
                switch (input[i])
                {
                    case '{':
                        depth++;
                        break;
                    case '}':
                        depth--;
                        break;
                }

                i++;
            }

            if (depth > 0)
            {
                // Unclosed brace - skip this malformed placeholder
                break;
            }

            var inner = input.Substring(start + 1, i - start - 2);
            Log.Debug($"Found placeholder {{{inner}}} at position {start}-{i - 1}");

            if (string.IsNullOrEmpty(inner))
            {
                continue;
            }

            string resolvedValue;
            var coordinateType = VariableCoordinateType.Invalid;

            // Method parsing
            if (char.IsUpper(inner[0]))
            {
                var tokens = new Tokenizer(scr).GetTokensFromLine(inner.ToCharArray(), -1);

                if (!new Contexter(scr).LinkAllTokens([tokens])
                    .HasErrored(out var linkError, out var contexts))
                {
                    if (contexts.Count == 1 && 
                        contexts.First() is MethodContext { Method: TextReturningMethod textMethod })
                    {
                        textMethod.Execute();

                        resolvedValue = textMethod.TextReturn ?? throw new DeveloperFuckupException(
                            $"Method '{textMethod.Name}' did not return a text value.");
                        
                        coordinateType = VariableCoordinateType.Method;
                    }
                    else
                    {
                        throw new MalformedConditionException($"Value '{inner}' is not a valid method.");
                    }
                }
                else
                {
                    throw new MalformedConditionException($"Value '{inner}' is not a valid method.");
                }
            }
            else if (scr.TryGetLiteralVariable(inner).WasSuccessful(out var variable))
            {
                resolvedValue = variable.Value() ?? throw new DeveloperFuckupException(
                    $"Variable '{variable.Name}' did not return a text value.");
                
                coordinateType = VariableCoordinateType.Variable;
            }
            else
            {
                throw new MalformedConditionException($"Value '{inner}' is not a valid variable.");
            }

            coordinates.Add(new VariableCoordinate
            {
                StartIndex = start,
                EndIndex = i - 1,
                PlaceholderText = $"{{{inner}}}",
                VariableName = inner,
                ResolvedValue = resolvedValue,
                Type = coordinateType
            });
        }

        return coordinates;
    }
    
    public static bool IsVariableSyntaxUsedInString(string value)
    {
        return Regex.Matches(value, "({.*?})").Count > 0;
    }

    public static bool IsVariableUsedInString(string value, Script scr, out Func<string> processedValueFunc)
    {
        processedValueFunc = null!;

        if (!IsVariableSyntaxUsedInString(value))
        {
            return false;
        }

        processedValueFunc = () => ReplaceVariablesInContaminatedString(value, scr);
        return true;
    }

    public static string[] SplitWithCharIgnoringVariables(string input, Func<char, bool> splitChar)
    {
        List<string> parts = [];
        var current = new StringBuilder();
        var depth = 0;

        foreach (var c in input)
        {
            switch (c)
            {
                case '{':
                    depth++;
                    current.Append(c);
                    continue;
                case '}':
                    depth = Math.Max(0, depth - 1);
                    current.Append(c);
                    continue;
            }

            if (splitChar(c) && depth == 0)
            {
                if (current.Length > 0)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                }

                continue;
            }

            current.Append(c);
        }

        if (current.Length > 0)
            parts.Add(current.ToString());

        return parts.ToArray();
    }

    public static bool IsValidVariableSyntax(string input)
    {
        return input.Length > 2 && input.StartsWith("{") && input.EndsWith("}");
    }

    public static TryGet<BaseMethod> TryParseMethod(string value, Script scr)
    {
        var rs = new ResultStacker($"Value '{value}' is not a valid method.");
        if (!char.IsUpper(value[0])) return rs.Add("Methods don't start with an uppercase letter.");
        
        var tokens = new Tokenizer(scr).GetTokensFromLine(value, -1);

        // todo: better system
        if (new Contexter(scr).LinkAllTokens([tokens])
            .HasErrored(out var linkError, out var contexts))
        {
            return rs.Add(linkError);
        }

        if (contexts.Count != 1 || contexts.First() is not MethodContext methodContext)
        {
            return rs;
        }

        return methodContext.Method;
    }
}