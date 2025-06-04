using System.Collections.Generic;
using System.Linq;
using SER.Helpers;
using SER.Helpers.Extensions;
using SER.Helpers.ResultStructure;
using SER.MethodSystem.ArgumentSystem;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.ScriptSystem.ContextSystem.Contexts;

public class MethodContext(MethodToken methodToken) : YieldingContext
{
    public readonly BaseMethod Method = methodToken.Method;
    public readonly MethodArgumentProcessor Processor = new(methodToken.Method);
    private int _providedArguments = 0;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        if (Processor.TryGetSkeleton(token, _providedArguments).HasErrored(out var error, out var skeleton))
            return TryAddTokenRes.Error(
                $"[Line {methodToken.LineNum}] Value '{token.RawRepresentation}' is not a valid argument: " +
                $"{error}");
        
        Method.Args.Add(skeleton);
        _providedArguments++;
        return TryAddTokenRes.Continue();
    }

    public override Result VerifyCurrentState()
    {
        return Result.Assert(_providedArguments >= Method.ExpectedArguments.Count(arg => !arg.IsOptional),
            $"Method '{Method.Name}' is missing required arguments: " +
            $"{", ".Join(Method.ExpectedArguments.Skip(_providedArguments).Select(arg => arg.Name))}");
    }

    protected override IEnumerator<float> Execute()
    {
        Log.Debug($"'{Method.Name}' method is now running..");

        switch (Method)
        {
            case Method stdAct:
                stdAct.Execute();
                yield break;
            case YieldingMethod yieldAct:
                var enumerator = yieldAct.Execute();
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
                yield break;
        }
    }
}