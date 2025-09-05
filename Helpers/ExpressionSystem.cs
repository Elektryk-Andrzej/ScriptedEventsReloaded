using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LabApi.Features.Console;
using NCalc;
using SER.Helpers.Extensions;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;
using SER.ScriptSystem.TokenSystem.Tokens.LiteralVariables;
using SER.VariableSystem;

namespace SER.Helpers;

public static class ExpressionSystem
{
    public static TryGet<bool> EvalCondition(string value, Script scr)
    {
        if (GetInternalResult(value, scr).HasErrored(out var err, out var result))
        {
            return err;
        }
        
        if (result is not bool boolRes)
        {
            return $"Expression '{value}' can't be interpreted as a true/false value!";
        }
        
        return boolRes;
    }
    
    public static TryGet<bool> EvalCondition(BaseToken[] tokens, Script scr)
    {
        if (GetInternalResult(tokens, scr).HasErrored(out var err, out var result))
        {
            return err;
        }
        
        if (result is not bool boolRes)
        {
            return $"Expression '{tokens.Select(t => t.RawRepresentation).JoinStrings(" ")}' can't be interpreted as a true/false value!";
        }
        
        return boolRes;
    }

    public static TryGet<string> EvalString(string value, Script scr)
    {
        if (GetInternalResult(value, scr).HasErrored(out var err, out var result))
        {
            return new(null, err);
        }

        return new(result.ToString(), null);
    }

    private static TryGet<object> GetInternalResult(string initValue, Script scr)
    {
        var rs = new ResultStacker($"Expression '{initValue}' is invalid.");
        var value = initValue.Replace("False", "false").Replace("True", "true");

        if (VariableParser.GetVariableCoordinatesInContaminatedString(value, scr)
            .HasErrored(out var err, out var coords))
        {
            return rs.Add(err);
        }
        
        Dictionary<string, object> variables = new();
        var id = 0;
        coords.Reverse();
        foreach (var coord in coords)
        {
            id++;
            var key = $"value{id}";
            
            value = value.Remove(coord.StartIndex, coord.Length)
                .Insert(coord.StartIndex, key);
            
            if (bool.TryParse(coord.ResolvedValue, out var boolRes))
            {
                variables[key] = boolRes;
            }
            else if (double.TryParse(coord.ResolvedValue, out var doubleRes))
            {
                variables[key] = doubleRes;
            }
            else
            {
                variables[key] = coord.ResolvedValue;
            }
        }

        var matches = Regex.Matches(value, @"\S+");
        foreach (Match match in matches.Cast<Match>().OrderByDescending(m => m.Index))
        {
            id++;
            if (match.Value is "==" or "!=" or ">" or "<" or ">=" or "<=" or "&&" or "||" or "+" or "-")
            {
                continue;
            }

            if (double.TryParse(match.Value, out _))
            {
                continue;
            }

            if (variables.ContainsKey(match.Value))
            {
                continue;
            }

            var key = $"value{id}";
            value = value.Remove(match.Index, match.Length)
                .Insert(match.Index, key);

            if (bool.TryParse(match.Value, out var boolRes))
            {
                variables[key] = boolRes;
            }
            else if (double.TryParse(match.Value, out var doubleRes))
            {
                variables[key] = doubleRes;
            }
            else
            {
                variables[key] = match.Value;
            }
        }

        return Evaluate(value,  variables, 0);
    }
    
    private static TryGet<object> GetInternalResult(BaseToken[] tokens, Script scr)
    {
        var initial = tokens.Select(t => t.RawRepresentation).JoinStrings(" ");
        var rs = new ResultStacker($"Expression '{initial}' is invalid.");

        string evalString = string.Empty;
        Dictionary<string, object> variables = new();
        uint tempVarId = 1;

        void AddToFinalString(string value)
        {
            evalString += $" {value}";
        }

        string TempVar(object value)
        {
            var tempVarName = $"var{tempVarId}";
            variables[tempVarName] = value;
            tempVarId++;
            return tempVarName;
        }
        
        foreach (var token in tokens)
        {
            switch (token)
            {
                case TextToken textToken:
                    AddToFinalString(TempVar(textToken.ValueWithoutBrackets));
                    continue;
                case LiteralVariableToken varToken:
                    if (varToken.TryGetValue(scr).HasErrored(out var err, out var value))
                    {
                        return rs.Add(err);
                    }

                    AddToFinalString(TempVar(value));
                    continue;
            }
            
            AddToFinalString(token.RawRepresentation);
        }
        
        return Evaluate(evalString, variables, 0);
    }

    private static TryGet<object> Evaluate(string value, Dictionary<string, object> variables, int tryNumber)
    {
        if (tryNumber > 10)
        {
            return $"Expression '{value}' is invalid.";
        }
        
        var expression = new Expression(value)
        {
            Parameters = variables
        };
        
        try
        {
            return expression.Evaluate();
        }
        catch (FormatException ex)
        {
            if (ex.Message != "String was not recognized as a valid Boolean.")
            {
                return $"{ex.GetType().Name}: {ex.Message}";
            }
            
            var boolVar = variables.FirstOrDefault(kvp => kvp.Value is bool);
            variables[boolVar.Key] = boolVar.Value.ToString();

            return Evaluate(value, variables, tryNumber + 1);
        }
        catch (Exception ex)
        {
            return $"{ex.GetType().Name}: {ex.Message}";
        }
    }
}