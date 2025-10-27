using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NCalc;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;

namespace SER.Helpers;

public static class NumericExpressionReslover
{
    public static TryGet<object> EvalString(string expression, Script scr)
    {
        Result mainErr = $"Expression '{expression}' is invalid.";
        
        if (Tokenizer.TokenizeLine(expression, scr, null)
            .HasErrored(out var err, out var tokens))
        {
            return mainErr + err;
        }
        
        return ParseExpression(tokens.ToArray());
    }
    
    public static TryGet<bool> EvalCondition(string expression, Script scr)
    {
        Result mainErr = $"Condition '{expression}' is invalid.";
        
        if (Tokenizer.TokenizeLine(expression, scr, null)
            .HasErrored(out var err, out var tokens))
        {
            return mainErr + err;
        }
        
        return EvalCondition(tokens.ToArray(), scr);
    }
    
    public static TryGet<bool> EvalCondition(BaseToken[] tokens, Script scr)
    {
        var result = ParseExpression(tokens);
        if (result.HasErrored(out var err, out var value))
            return err;

        if (value is bool bool1)
        {
            return bool1;
        }

        if (bool.TryParse(value.ToString(), out var bool2))
        {
            return bool2;
        }

        return "Expression did not evaluate to a true/false value.";
    }
    
    public static TryGet<object> ParseExpression(BaseToken[] tokens)
    {
        var initial = tokens.Select(t => t.RawRep).JoinStrings(" ");
        Result mainErr = $"Expression '{initial}' is invalid.";

        var variables = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        var sb = new StringBuilder();
        int tempId = 0;

        foreach (var token in tokens)
        {
            Log.D($"parsing token {token.RawRep} ({token.GetType().Name})");
            switch (token)
            {
                case IValueCapableToken<LiteralValue> literalValue:
                {
                    if (literalValue.ExactValue.HasErrored(out var err, out var resolved))
                        return mainErr + err;

                    var tmp = MakeTempName();
                    variables[tmp] = resolved.BaseValue;
                    AppendRaw(tmp);
                    continue;
                }
                case ParenthesesToken parentheses:
                {
                    if (parentheses.TryGetTokens().HasErrored(out var tokenizeError, out var tokensInParentheses))
                    {
                        return mainErr + tokenizeError;
                    }
                
                    if (ParseExpression(tokensInParentheses).HasErrored(out var conditonError, out var value))
                    {
                        return mainErr + conditonError;
                    }
                
                    var tmp = MakeTempName();
                    variables[tmp] = value;
                    AppendRaw(tmp);
                    continue;
                }
                default:
                    AppendRaw(token.RawRep);
                    break;
            }
        }

        // Now we have the expression string and a variables dictionary.
        return Evaluate(sb.ToString(), variables, mainErr);

        string MakeTempName()
        {
            return "value" + (tempId++).ToString(CultureInfo.InvariantCulture);
        }

        void AppendRaw(string s)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(s);
        }
    }

    // --- Evaluate helper that sets typed params and uses EvaluateParameter ---
    private static TryGet<object> Evaluate(string exprString, Dictionary<string, object> variables, Result rs)
    {
        var expression = new Expression(exprString, EvaluateOptions.None)
        {
            // Supply our typed parameter dictionary:
            Parameters = new Dictionary<string, object>(variables, StringComparer.OrdinalIgnoreCase)
        };

        // As a safe fallback, respond to EvaluateParameter requests by resolving from variables
        expression.EvaluateParameter += (name, args) =>
        {
            if (variables.TryGetValue(name, out var val))
            {
                args.Result = val;
                return;
            }

            // if not found, return a helpful error via exception. NCalc will bubble it.
            throw new ArgumentException($"Unknown identifier '{name}' in expression.");
        };

        try
        {
            var result = expression.Evaluate();
            return new TryGet<object>(result, null); // adapt to your TryGet ctor
        }
        catch (Exception ex)
        {
            // Keep error messages friendly and actionable
            return rs + $"{ex.GetType().Name}: {ex.Message}";
        }
    }

    // (Keep the rest of the public API: EvalCondition, EvalString etc. that call GetInternalResult)
}