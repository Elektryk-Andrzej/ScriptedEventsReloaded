using System.Text.RegularExpressions;
using NCalc;
using SER.ScriptSystem;
using SER.VariableSystem;

namespace SER.Helpers;

public static class Condition
{
    public static TryGet<bool> TryEval(string value, Script scr)
    {
        value = VariableParser.ReplaceVariablesInContaminatedString(value, scr);
        value = value.Replace("False", "false").Replace("True", "true");
        
        var expression = new Expression(value);
        
        var matches = Regex.Matches(value, @"\w+");
        foreach (Match match in matches)
        {
            if (double.TryParse(match.Value, out _))
            {
                continue;
            }
            
            expression.Parameters[match.Value] = match.Value;
        }
        
        var result = expression.Evaluate();
        if (result is not bool boolRes)
        {
            return $"Result of condition '{value}' is not a true/false value.";
        }
        
        return boolRes;
    }
}