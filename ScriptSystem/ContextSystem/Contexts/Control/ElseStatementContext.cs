using System.Collections.Generic;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.Control;

public class ElseStatementContext : TreeContext, ITreeExtender
{
    public IExtendableTree.ControlMessage Extends => IExtendableTree.ControlMessage.DidntExecute;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Error("There should be no arguments after `else` keyword");
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    public override IEnumerator<float> Execute()
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