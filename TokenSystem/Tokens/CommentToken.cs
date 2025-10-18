using System.Linq;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;

namespace SER.TokenSystem.Tokens;

public class CommentToken : BaseToken
{
    protected override Result InternalParse(Script scr)
    {
        if (RawRepresentation.FirstOrDefault() == '#') return true;
        
        return "Value is not a comment.";
    }
}