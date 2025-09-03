using System.Collections.Generic;
using System.Linq;
using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Contexts.Control;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Structures;

namespace SER.ScriptSystem.ContextSystem;

/// <summary>
/// Responsible for joining file tokens together into contexts for execution.
/// </summary>
public class Contexter(Script script)
{
    private readonly List<Context> _contexts = [];
    private readonly List<TreeContext> _processingTreeContexts = [];

    public TryGet<List<Context>> LinkAllTokens(List<ScriptLine> lines)
    {
        var rs = new ResultStacker($"Script {script.Name} cannot compile.");

        foreach (var line in lines)
        {
            if (HandleLine(line).HasErrored(out var error, out var context))
            {
                return rs.Add(error);
            }
            
            if (context is null) continue;

            if (TryAddResult(context, line.LineNumber).HasErrored(out var addError))
            {
                return rs.Add(addError);
            }
        }

        Log.Debug($"Contexting script {script.Name} has ended");
        return _contexts;
    }

    private Result TryAddResult(Context context, int lineNum)
    {
        var rs = new ResultStacker($"Invalid context {context} in line {lineNum}.");

        if (context is EndTreeContext)
        {
            if (_processingTreeContexts.Count == 0) 
                return rs.Add("There is no statement to end with the `end` keyword!");

            _processingTreeContexts.RemoveAt(_processingTreeContexts.Count - 1);
            return true;
        }

        var currentTree = _processingTreeContexts.LastOrDefault();
        if (context is ElseStatementContext elseStatement)
        {
            if (currentTree is not IfStatementContext ifStatement)
            {
                return rs.Add("The 'else' is not right after the 'if' statement.");
            }
            
            _processingTreeContexts.RemoveAt(_processingTreeContexts.Count - 1);
            _processingTreeContexts.Add(elseStatement);
            ifStatement.ElseStatement = elseStatement;
            return true;
        }

        if (context.VerifyCurrentState().HasErrored(out var error)) 
            return rs.Add(error);

        if (currentTree is not null)
        {
            Log.Debug($"Adding finished context {context} to tree context {currentTree}");
            currentTree.Children.Add(context);
            context.ParentContext = currentTree;
        }
        else
        {
            Log.Debug($"Adding finished context {context} to main collection");
            _contexts.Add(context);
        }

        if (context is TreeContext treeContext) _processingTreeContexts.Add(treeContext);

        Log.Debug($"Line {lineNum} has been contexted to {context}");
        return true;
    }

    private TryGet<Context?> HandleLine(ScriptLine line)
    {
        Log.Debug($"Handling line {line.LineNumber}:");
        var rs = new ResultStacker($"Line {line.LineNumber} cannot execute");

        var firstToken = line.Tokens.FirstOrDefault();
        if (firstToken == null)
        {
            Log.Debug($"Line {line.LineNumber} is empty");
            return null as Context;
        }

        if (firstToken is not ContextableToken contextable)
        {
            Log.Warn(script, $"Line {line.LineNumber} does not start with a contextable token");
            return null as Context;
        }
        
        if (contextable.TryGetResultingContext().HasErrored(out var contextError, out var context))
            return rs.Add(contextError);

        foreach (var token in line.Tokens.Skip(1))
        {
            if (HandleCurrentContext(token, context, out var endLineContexting).HasErrored(out var errorMsg))
                return rs.Add(errorMsg);

            if (endLineContexting) break;
        }

        return context;
    }

    private static Result HandleCurrentContext(BaseToken token, Context context, out bool endLineContexting)
    {
        var rs = new ResultStacker($"Cannot add token {token} to context {context}");
        Log.Debug($"Handling token {token} in context {context}");

        var result = context.TryAddToken(token);
        if (result.HasErrored)
        {
            endLineContexting = true;
            return rs.Add(result.ErrorMessage);
        }

        if (result.ShouldContinueExecution)
        {
            endLineContexting = false;
            return true;
        }

        endLineContexting = true;
        return true;
    }
}