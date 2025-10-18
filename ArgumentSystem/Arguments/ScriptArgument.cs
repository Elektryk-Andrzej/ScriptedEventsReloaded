using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ScriptSystem.Structures;
using SER.TokenSystem.Tokens;

namespace SER.ArgumentSystem.Arguments;

/// <summary>
/// Represents a script object argument used in a method.
/// </summary>
public class ScriptArgument(string name) : Argument(name)
{
    public override string InputDescription => "Name of an existing script";

    [UsedImplicitly]
    public DynamicTryGet<Script> GetConvertSolution(BaseToken token)
    {
        var value = token.GetBestTextRepresentation(Script);
        if (GetScript(value).HasErrored(out var error))
        {
            return new(error);
        }

        return new(() => DynamicSolver(value));
    }

    private static TryGet<Script> DynamicSolver(string value)
    {
        if (GetScript(value).HasErrored(out var error, out var script))
        {
            return error;
        }

        return script;
    }

    private static TryGet<Script> GetScript(string scriptIdentification)
    {
        if (!Script.CreateByPath(scriptIdentification, ServerConsoleExecutor.Instance)
                .HasErrored(out _, out var scrByPath))
        {
            return scrByPath;
        }
        
        if (!Script.CreateByScriptName(scriptIdentification, ServerConsoleExecutor.Instance)
                .HasErrored(out _, out var scrByName))
        {
            return scrByName;
        }

        return $"Script '{scriptIdentification}' doesn't exist.";
    }
}