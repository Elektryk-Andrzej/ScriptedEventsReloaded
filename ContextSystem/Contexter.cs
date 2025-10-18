using System.Collections.Generic;
using System.Linq;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Contexts.Control;
using SER.ContextSystem.Structures;
using SER.Helpers;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.TokenSystem.Structures;
using SER.TokenSystem.Tokens;

namespace SER.ContextSystem;

/// <summary>
/// Responsible for joining tokens from a line together into contexts for execution.
/// </summary>
public static class Contexter
{
    public static TryGet<Context[]> ContextLines(Line[] lines, Script scr)
    {
        List<StatementContext> statementStack = [];
        List<Context> contexts = [];
        
        foreach (var line in lines)
        {
            Result mainErr = $"Line {line.LineNumber} cannot compile.";
            if (ContextLine(line.Tokens, line.LineNumber, scr)
                .HasErrored(out var error, out var context))
            {
                return mainErr + error;
            }
            
            if (context is null) continue;
            
            if (TryAddResult(context, line.LineNumber, statementStack, contexts).HasErrored(out var addError))
            {
                return mainErr + addError;
            }
        }
        
        return contexts.ToArray();
    }

    private static Result TryAddResult(
        Context context,
        uint lineNum, 
        List<StatementContext> statementStack, 
        List<Context> contexts
    ) {
        Result rs = $"Invalid context {context} in line {lineNum}.";

        if (context is EndStatementContext)
        {
            if (statementStack.Count == 0) 
                return rs + "There is no statement to close with the 'end' keyword!";

            statementStack.RemoveAt(statementStack.Count - 1);
            return true;
        }

        var currentStatement = statementStack.LastOrDefault();
        if (context is StatementContext treeExtenderContext and IStatementExtender treeExtenderInfo)
        {
            if (currentStatement is null)
            {
                return rs + "There is no statement to extend.";
            }

            if (currentStatement is not IExtendableStatement extendable)
            {
                return rs + "The statement to extend is not extendable.";
            }
            
            if (!extendable.AllowedSignals.HasFlag(treeExtenderInfo.Extends))
            {
                return rs + "The statement to extend does not support this type of extension.";
            }

            extendable.RegisteredSignals[treeExtenderInfo.Extends] = treeExtenderContext.Run;
            statementStack.RemoveAt(statementStack.Count - 1);
            statementStack.Add(treeExtenderContext);
            return true;
        }

        if (context.VerifyCurrentState().HasErrored(out var error)) 
            return rs + error;

        if (currentStatement is not null)
        {
            Log.Debug($"Adding finished context {context} to tree context {currentStatement}");
            currentStatement.Children.Add(context);
            context.ParentContext = currentStatement;
        }
        else
        {
            Log.Debug($"Adding finished context {context} to main collection");
            contexts.Add(context);
        }

        if (context is StatementContext treeContext) 
            statementStack.Add(treeContext);

        Log.Debug($"Line {lineNum} has been contexted to {context}");
        return true;
    }

    public static TryGet<Context?> ContextLine(BaseToken[] tokens, uint? lineNum, Script scr)
    {
        Result rs = $"Line {(lineNum.HasValue ? $"{lineNum.Value} " : "")}cannot execute";
        
        var firstToken = tokens.FirstOrDefault();
        if (firstToken is null) return null as Context;
        
        if (firstToken is not IContextableToken contextable)
        {
            return rs + "The beginning of the line is incorrectly strucutred.";
        }
        
        if (contextable.TryGetContext(scr).HasErrored(out var contextError, out var context))
            return rs + contextError;

        foreach (var token in tokens.Skip(1))
        {
            if (HandleCurrentContext(token, context, out var endLineContexting).HasErrored(out var errorMsg))
                return rs + errorMsg;

            if (endLineContexting) break;
        }

        return context;
    }

    private static Result HandleCurrentContext(BaseToken token, Context context, out bool endLineContexting)
    {
        Result rs = $"Cannot add token {token} to context {context}";
        Log.Debug($"Handling token {token} in context {context}");

        var result = context.TryAddToken(token);
        if (result.HasErrored)
        {
            endLineContexting = true;
            return rs + result.ErrorMessage;
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