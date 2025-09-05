

using System;
using System.Collections.Generic;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.Control.Loops;

public class ForeachLoopContext : StatementContext, IExtendableStatement, IKeywordContext
{
    private readonly ResultStacker _rs = new("Cannot create 'foreach' loop.");
    private bool _skipChild = false;
    
    private PlayerVariableToken? _iterationVariable;
    private bool _usedInKeyword = false;
    private PlayerVariableToken? _iterable;

    public string Keyword => "foreach";
    public string Description =>
        "Repeats its body for each player in the player variable, assigning it its own custom variable.";
    public string[] Arguments => ["[player variable]", "in", "[player variable]"];

    public IExtendableStatement.Signal AllowedSignals => IExtendableStatement.Signal.DidntExecute;
    public Dictionary<IExtendableStatement.Signal, Func<IEnumerator<float>>> RegisteredSignals { get; } = new();

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        if (_iterationVariable is null)
        {
            if (token is not PlayerVariableToken varToken)
            {
                return TryAddTokenRes.Error("Foreach loop expects to have a player variable as its first argument.");
            }
            
            _iterationVariable = varToken;
            return TryAddTokenRes.Continue();
        }

        if (_usedInKeyword is false)
        {
            if (token.RawRepresentation != "in")
            {
                return TryAddTokenRes.Error("Foreach loop expects to have 'in' keyword as its second argument.");
            }
            
            _usedInKeyword = true;
            return TryAddTokenRes.Continue();
        }

        if (token is not PlayerVariableToken iterable)
        {
            return TryAddTokenRes.Error("Foreach loop expects to have a player variable as its third argument.");
        }
        
        _iterable = iterable;
        return TryAddTokenRes.End();
    }

    public override Result VerifyCurrentState()
    {
        return Result.Assert(
            _iterationVariable is not null && _iterable is not null && _usedInKeyword,
            _rs.Add("Missing required arguments."));
    }

    public override IEnumerator<float> Execute()
    {
        // todo
        yield break;
    }
}