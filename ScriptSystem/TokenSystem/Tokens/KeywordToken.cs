using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.TokenSystem.Tokens;

public class KeywordToken : ContextableToken
{
    public static Dictionary<string, (Type type, string description, string[] arguments)> KeywordInfo = new();
    
    public static void RegisterKeywords()
    {
        KeywordInfo = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IKeywordContext).IsAssignableFrom(t) && typeof(Context).IsAssignableFrom(t))
            .Select(t => (t, Activator.CreateInstance(t) as IKeywordContext))
            .Cast<(Type type, IKeywordContext keyword)>()
            .ToDictionary(k => k.keyword.Keyword, k => (k.type, k.keyword.Description, k.keyword.Arguments));
    }
    
    public override bool EndParsingOnChar(char c, out BaseToken? replaceToken)
    {
        replaceToken = null;
        return char.IsWhiteSpace(c);
    }

    public override Result IsValidSyntax()
    {
        return true;
    }

    public override TryGet<Context> TryGetResultingContext()
    {
        var info = (Script, LineNum);
        if (!KeywordInfo.TryGetValue(RawRepresentation.ToLower(), out var keywordInfo))
        {
            return $"Value '{RawRepresentation}' is not a keyword.";
        }
        
        return Context.Create(keywordInfo.type, info);
    }
}