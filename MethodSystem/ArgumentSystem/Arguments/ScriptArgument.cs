using SER.Helpers;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a script object argument used in a method.
/// </summary>
public class ScriptArgument(string name) : BaseMethodArgument(name)
{
    public override OperatingValue Input => OperatingValue.Script;

    public override string? AdditionalDescription => null;

    public ArgumentEvaluation<Script> GetConvertSolution(BaseToken token)
    {
        var value = token.GetValue();
        if (GetScript(value).HasErrored(out var error))
        {
            return new(Rs.Add(error));
        }

        return new(() => DynamicSolver(value));
    }

    private ArgumentEvaluation<Script>.EvalRes DynamicSolver(string value)
    {
        if (GetScript(value).HasErrored(out var error, out var script))
        {
            return Rs.Add(error);
        }

        return script;
    }

    private static TryGet<Script> GetScript(string scriptIdentification)
    {
        if (!Script.CreateByPath(scriptIdentification).HasErrored(out _, out var scrByPath))
        {
            return scrByPath;
        }
        
        if (!Script.CreateByScriptName(scriptIdentification).HasErrored(out _, out var scrByName))
        {
            return scrByName;
        }

        return $"Script '{scriptIdentification}' doesn't exist.";
    }
}