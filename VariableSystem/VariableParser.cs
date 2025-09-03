using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultStructure;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem;
using SER.ScriptSystem.ContextSystem;
using SER.ScriptSystem.ContextSystem.Contexts;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.Tokens.LiteralVariables;
using SER.VariableSystem.Structures;

namespace SER.VariableSystem;

/// <summary>
/// Provides methods to parse and process variables within strings.
/// This includes replacing placeholders with actual variable values,
/// extracting variable coordinates, checking for variable syntax in strings,
/// and parsing methods or expressions involving variables.
/// Probably one of the most inefficient classes in the entire project.
/// </summary>
public static class VariableParser
{
    private const string PlayerVariableRegex = @"@[a-z][a-zA-Z0-9]+\.\w+";

    public static string ReplaceVariablesInContaminatedString(string input, Script scr)
    {
        var result = new StringBuilder();
        var i = 0;

        // player variables
        foreach (var match in Regex.Matches(input, PlayerVariableRegex).Cast<Match>().Reverse())
        {
            if (!TryGetPlayerPropertyInfo(match.Value, scr).WasSuccessful(out var property))
            {
                continue;
            }
            
            input = input.Substring(0, match.Index) 
                    + property
                    + input.Substring(match.Index + match.Length);
        }
        
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
            if (string.IsNullOrEmpty(inner))
            {
                AddUnparsed();
                continue;
            }

            var tokenizedLine = new Tokenizer(scr).GetTokensFromLine(inner.ToCharArray(), 0);
            
            // method parsing
            if (char.IsUpper(inner[0]))
            {
                if (!new Contexter(scr).LinkAllTokens([tokenizedLine]).WasSuccessful(out var contexts)
                    || contexts.Count != 1
                    || contexts.First() is not MethodContext { Method: TextReturningMethod textMethod })
                {
                    AddUnparsed();
                    continue;
                }

                textMethod.Execute();
                result.Append(textMethod.TextReturn);
                continue;
            }
            
            if (scr.TryGetLiteralVariable(inner).WasSuccessful(out var variable))
            {
                result.Append(variable.Value());
                continue;
            }
            
            AddUnparsed();
            continue;

            void AddUnparsed()
            {
                result.Append($"{{{inner}}}");
            }
        }

        return result.ToString();
    }
    
    public static TryGet<List<ValueCoordinate>> GetVariableCoordinatesInContaminatedString(
        string input,
        Script scr
    )
    {
        var rs = new ResultStacker("Isolating variables from a contaminated string failed.");
        
        var coordinates = new List<ValueCoordinate>();
        var index = -1;
        while (index+1 < input.Length)
        {
            index++;
            if (input[index] == '@')
            {
                var endIndex = input.IndexOf(' ', index) - 1;

                string varName;
                if (endIndex == -2)
                {
                    endIndex = input.Length - 1;
                    varName = input.Substring(index);
                }
                else
                {
                    varName = input.Substring(index, endIndex - index + 1);
                }

                if (TryGetPlayerPropertyInfo(varName, scr).HasErrored(out var error, out var value))
                {
                    return rs.Add(error);
                }
                
                coordinates.Add(new ValueCoordinate
                {
                    StartIndex = index,
                    EndIndex = endIndex,
                    OriginalValue = varName,
                    ResolvedValue = value,
                });
                continue;
            }
            
            if (input[index] != '{')
            {
                continue;
            }
            
            var varSyntaxStart = index;
            index++;
            var depth = 1;
            while (index < input.Length && depth > 0)
            {
                switch (input[index])
                {
                    case '{':
                        depth++;
                        break;
                    case '}':
                        depth--;
                        break;
                }

                index++;
            }
            
            if (depth > 0)
            {
                return rs.Add(
                    $"There is an unclosed variable syntax in the string: '{input.Substring(varSyntaxStart)}'.");
            }

            var inner = input.Substring(varSyntaxStart + 1, index - varSyntaxStart - 2);
            if (string.IsNullOrEmpty(inner))
            {
                continue;
            }

            string resolvedValue;
            // Method parsing
            if (char.IsUpper(inner[0]))
            {
                var tokens = new Tokenizer(scr).GetTokensFromLine(inner.ToCharArray(), -1);

                if (!new Contexter(scr).LinkAllTokens([tokens]).WasSuccessful(out var contexts) ||
                    contexts.Count != 1 || 
                    contexts.First() is not MethodContext { Method: TextReturningMethod textMethod })
                {
                    return rs.Add($"Value '{inner}' is not a valid method.");
                }

                textMethod.Execute();

                resolvedValue = textMethod.TextReturn ?? throw new AndrzejFuckedUpException(
                    $"Method '{textMethod.Name}' did not return a text value.");
            }
            else if (scr.TryGetLiteralVariable(inner).WasSuccessful(out var variable))
            {
                resolvedValue = variable.Value() ?? throw new AndrzejFuckedUpException(
                    $"Variable '{variable.Name}' did not return a text value.");
            }
            else
            {
                return rs.Add($"Value '{inner}' is not a valid variable.");
            }
            
            coordinates.Add(new ValueCoordinate
            {
                StartIndex = varSyntaxStart,
                EndIndex = index - 1,
                OriginalValue = inner,
                ResolvedValue = resolvedValue,
            });
        }

        return coordinates;
    }
    
    public static bool IsValueSyntaxUsedInString(string value)
    {
        if (Regex.Matches(value, "({.*?})").Count > 0)
        {
            return true;
        }

        if (Regex.Matches(value, PlayerVariableRegex).Count > 0)
        {
            return true;
        }
        
        return false;
    }

    public static bool IsValueSyntaxUsedInString(string value, Script scr, out Func<string> processedValueFunc)
    {
        processedValueFunc = null!;

        if (!IsValueSyntaxUsedInString(value))
        {
            return false;
        }

        processedValueFunc = () => ReplaceVariablesInContaminatedString(value, scr);
        return true;
    }

    public static TryGet<Method> TryParseMethod(string value, Script scr)
    {
        var rs = new ResultStacker($"Value '{value}' is not a valid method.");
        if (!char.IsUpper(value[0])) return rs.Add("Methods must start with an uppercase letter.");
        
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

    private static TryGet<string> TryGetPlayerPropertyInfo(string value, Script scr)
    {
        var error = $"Value '{value}' is not a valid player property access.";
        var parts = value.Split('.');
        if (parts.Len() != 2)
        {
            return new(null, error);
        }

        var varName = parts.First();
        if (!varName.StartsWith("@"))
        {
            return new(null, error);
        }
        
        varName = varName.Substring(1);
        if (!scr.TryGetPlayerVariable(varName).WasSuccessful(out var variable) 
            || variable.Players.Len() != 1)
        {
            return new(null, error);
        }

        var key = parts.Last().ToLower();
        if (!PlayerPropertyAccessToken.CaseInsensitiveAccessiblePlayerProperties.TryGetValue(key, out var info))
        {
            return new(null, error);
        }

        return new(info.getAction(variable.Players.First()), null);
    }
}