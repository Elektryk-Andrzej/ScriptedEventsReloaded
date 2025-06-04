using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NCalc;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem;
using SER.VariableSystem;
using SER.VariableSystem.Structures;

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
        var coords = VariableParser.GetVariableCoordinatesInContaminatedString(value, scr);
        coords.Reverse();
        
        var failedCoord = coords.FirstOrDefault(c => c.Type == VariableCoordinateType.Invalid);
        if (failedCoord is not null)
        {
            return rs.Add($"Value '{failedCoord.VariableName}' is not valid.");
        }
        
        Dictionary<string, object> variables = new();
        var id = 0;
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
            else
            {
                variables[key] = coord.ResolvedValue;
            }
        }
        
        try
        {
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
                else
                {
                    variables[key] = match.Value;
                }
            }
            
            var expression = new Expression(value)
            {
                Parameters = variables
            };
            
            return expression.Evaluate();
        }
        catch (Exception ex)
        {
            return rs.Add($"{ex.GetType().Name}: {ex.Message}");
        }
    }
}