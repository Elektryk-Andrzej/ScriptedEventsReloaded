using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Slices;
using SER.TokenSystem.Structures;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class BaseToken
{
    public string RawRepresentation { get; private set; } = null!;
    protected Slice Slice { get; private set; } = null!;
    protected Script Script { get; private set; } = null!;
    protected uint? LineNum { get; private set; } = null;

    public Result TryInit(Slice slice, Script script, uint? lineNum)
    {
        RawRepresentation = slice.RawRepresentation;
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
        if (this is ILiteralValueToken literalValueToken && script is not null)
        {
            if (!literalValueToken.GetLiteralValue(script).HasErrored(out _, out var result))
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

    public TryGet<T> TryGetValue<T>() where T : LiteralValue
    {
        Result mainErr = $"Value '{RawRepresentation}' cannot be intrepreted as a '{typeof(T).Name}' value.";
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (this is ValueToken<T> valueToken)
        {
            return valueToken.Value;
        }

        if (this is ILiteralValueToken literalValueToken)
        {
            if (literalValueToken.GetLiteralValue(Script)
                .HasErrored(out var err, out var resolved))
            {
                return mainErr + err;
            }
            
            if (typeof(T) == typeof(string))
            {
                return TryGet<T>.Success((T)resolved);
            }
        }
        
        if (this is ParenthesesToken parenthesesToken)
        {
            if (parenthesesToken.ParseExpression().HasErrored(out var err, out var result))
            {
                return mainErr + err;
            }

            if (result is T correctValue)
            {
                return correctValue;
            }

            var closestValue = LiteralValue.GetValueFromObject(result);
            if (closestValue is T correctValue2)
            {
                return correctValue2;           
            }

            return
                $"Expression '{parenthesesToken.RawRepresentation}' parsed to a {result.GetType().GetAccurateName()} '{result}', " +
                $"which is not a '{typeof(T).Name}' value.";
        }
        
        return mainErr;
    }
}