using System.Linq;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Contexts.VariableDefinition;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Structures;
using SER.ValueSystem;
using SER.VariableSystem.Variables;

namespace SER.TokenSystem.Tokens;

public class LiteralVariableToken : BaseToken, IContextableToken, ILiteralValueToken
{
    public char? Prefix { get; private set; } = null;
    public string Name { get; private set; } = null!;
    public LiteralVariable? Variable => TryGetVariable().WasSuccessful(out var variable) ? variable : null;
    
    protected override Result InternalParse(Script scr)
    {
        if (RawRepresentation.Length < 2)
        {
            return "Variable name is too short.";
        }

        if (RawRepresentation.FirstOrDefault() != '$')
        {
            return "Variable is missing '$' prefix.";
        }
        
        Prefix = RawRepresentation[0];
        Name = RawRepresentation.Substring(1);

        if (!Name.All(char.IsLetter))
        {
            return "Variable name must only contain letters.";
        }

        if (!char.IsLower(Name.First()))
        {
            return "Variable name must start with a lowercase letter.";
        }
        
        return true;
    }

    public TryGet<LiteralVariable> TryGetVariable()
    {
        return Script.TryGetLiteralVariable(this);
    }

    public TryGet<Context> TryGetContext(Script scr)
    {
        return new LiteralVariableDefinitionContext(this)
        {
            Script = scr,
            LineNum = LineNum
        };
    }

    public TryGet<LiteralValue> GetLiteralValue(Script scr)
    {
        if (TryGetVariable().HasErrored(out var error, out var variable))
        {
            return error;
        }
        
        return variable.BaseValue;
    }
}