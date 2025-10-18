using System;
using System.Linq;
using System.Reflection;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Structures;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Structures;

namespace SER.TokenSystem.Tokens;

public class KeywordToken : BaseToken, IContextableToken
{
    private Type? _keywordType = null;
    
    public static readonly Type[] KeywordTypes = Assembly.GetExecutingAssembly().GetTypes()
        .Where(t => 
            t.IsClass && 
            !t.IsAbstract && 
            typeof(IKeywordContext).IsAssignableFrom(t) &&
            typeof(Context).IsAssignableFrom(t)
        )
        .ToArray();
    
    protected override Result InternalParse(Script scr)
    {
        _keywordType = KeywordTypes.FirstOrDefault(
            keyword => keyword.CreateInstance<IKeywordContext>().KeywordName == RawRepresentation);

        if (_keywordType is not null)
        {
            return true;
        }

        return $"There is no keyword called '{RawRepresentation}'.";
    }

    public TryGet<Context> TryGetContext(Script scr)
    {
        return Context.Create(_keywordType!, (scr, LineNum));
    }
}