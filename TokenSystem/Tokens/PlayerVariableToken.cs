using System.Linq;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.VariableSystem.Variables;

namespace SER.TokenSystem.Tokens;

public class PlayerVariableToken : BaseToken
{
    public string Name { get; private set; } = null!;
    
    protected override Result InternalParse(Script scr)
    {
        var name = Slice.RawRepresentation;
        if (name.Length <= 1)
        {
            return "Variable name is too short.";
        }

        if (name.FirstOrDefault() != '@')
        {
            return "Player variable must start with '@'.";
        }
        
        name = name.Substring(1);

        if (!name.All(char.IsLetter))
        {
            return "Variable name must only contain letters (excluding @).";
        }

        if (!char.IsLower(name.First()))
        {
            return "Variable name must start with a lowercase letter (excluding @).";
        }
        
        Name = Slice.RawRepresentation.Substring(1);
        return true;
    }
    
    public TryGet<PlayerVariable> TryGetVariable()
    {
        return Script.TryGetPlayerVariable(this);
    }
}