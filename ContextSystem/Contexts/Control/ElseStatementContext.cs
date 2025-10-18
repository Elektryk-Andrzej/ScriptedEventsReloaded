using System.Collections.Generic;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Structures;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ContextSystem.Contexts.Control;

public class ElseStatementContext : StatementContext, IStatementExtender, IKeywordContext
{
    public string KeywordName => "else";
    public string Description =>
        "If the statement above it didn't execute, 'else' statement will execute instead.";
    public string[] Arguments => [];
    
    public IExtendableStatement.Signal Extends => IExtendableStatement.Signal.DidntExecute;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Error("There should be no arguments after `else` keyword");
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    protected override IEnumerator<float> Execute()
    {
        foreach (var child in Children)
        {
            switch (child)
            {
                case YieldingContext yielding:
                {
                    var enumerator = yielding.Run();
                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }

                    break;
                }
                case StandardContext standard:
                    standard.Run();
                    break;
                default:
                    throw new AndrzejFuckedUpException();
            }
        }
    }
}