using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Slices;
using SER.TokenSystem.Tokens.Interfaces;
using SER.TokenSystem.Tokens.Variables;
using SER.ValueSystem;
using SER.VariableSystem.Variables;

namespace SER.TokenSystem.Tokens;

public class BaseToken
{
    public string RawRep { get; private set; } = null!;
    protected Slice Slice { get; private set; } = null!;
    protected Script Script { get; private set; } = null!;
    protected uint? LineNum { get; private set; } = null;
    
    public Result TryInit(Slice slice, Script script, uint? lineNum)
    {
        RawRep = slice.RawRepresentation;
        Slice = slice;
        Script = script;
        LineNum = lineNum;
        return InternalParse(script);
    }

    protected virtual Result InternalParse(Script scr)
    {
        return true;
    }
    
    public string GetBestTextRepresentation(Script? script)
    {
        if (this is IValueCapableToken<LiteralValue> literalValueToken && script is not null)
        {
            if (!literalValueToken.ExactValue.HasErrored(out _, out var result))
            {
                return result.ToString();
            }
        }
        else if (this is TextToken textToken)
        {
            return textToken.Value;
        }

        return Slice.RawRepresentation;
    }

    public override string ToString()
    {
        return GetType().Name;
    }

    public TryGet<T> TryGetLiteralValue<T>() where T : LiteralValue
    {
        Result mainErr = $"Value '{RawRep}' ({GetType().Name}) cannot be intrepreted as a '{typeof(T).Name}' value.";
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (this is LiteralValueToken<T> valueToken)
        {
            return valueToken.Value;
        }

        if (this is IValueCapableToken<LiteralValue> literalValueToken)
        {
            if (literalValueToken.ExactValue.HasErrored(out var err, out var value))
            {
                return mainErr + err;
            }

            if (value is T tValue)
            {
                return tValue;
            }

            return $"The value returned by '{RawRep}' was of type '{value.GetType().Name}', " +
                   $"but a '{typeof(T).Name}' value was expected.";
        }
        
        if (this is ParenthesesToken parenthesesToken)
        {
            if (parenthesesToken.ParseExpression().HasErrored(out var err, out var result))
            {
                return mainErr + err;
            }
            
            if (Value.Parse(result) is T correctValue)
            {
                return correctValue;           
            }

            return $"Expression '{parenthesesToken.RawRep}' parsed to a {result.GetType().GetAccurateName()} " +
                   $"'{result}', which is not a '{typeof(T).Name}' value.";
        }

        if (this is VariableToken varToken)
        {
            if (varToken.TryGetVariable().HasErrored(out var error, out var variable))
            {
                return mainErr + error;
            }

            if (variable is not LiteralVariable litVariable)
            {
                return $"Variable '{varToken.RawRep}' is not a literal variable, but a {variable.GetType().Name}"; 
            }

            if (litVariable.Value is not T tValue)
            {
                return $"Value of variable '{varToken.RawRep}' is not a '{typeof(T).Name}' value, " +
                       $"but a {litVariable.Value.GetType().Name}.";
            }

            return tValue;
        }
        
        return mainErr;
    }
}