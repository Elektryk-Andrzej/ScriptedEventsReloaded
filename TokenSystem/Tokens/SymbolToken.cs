using System.Linq;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;

namespace SER.TokenSystem.Tokens;

public class SymbolToken : BaseToken
{
    public bool IsJoker => RawRep == "*";
    
    protected override Result InternalParse(Script scr)
    {
        if (RawRep.All(c => char.IsSymbol(c) || char.IsPunctuation(c)))
        {
            return true;
        }
        
        return $"Value '{RawRep}' is not a symbol.";
    }
}