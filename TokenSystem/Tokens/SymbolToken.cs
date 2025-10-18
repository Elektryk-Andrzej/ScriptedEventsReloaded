using System.Linq;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class SymbolToken : ValueToken<TextValue>
{
    public bool IsJoker => RawRepresentation == "*";
    
    protected override Result InternalParse(Script scr)
    {
        if (RawRepresentation.All(c => char.IsSymbol(c) || char.IsPunctuation(c)))
        {
            return true;
        }
        
        return $"Value '{RawRepresentation}' is not a symbol.";
    }
}