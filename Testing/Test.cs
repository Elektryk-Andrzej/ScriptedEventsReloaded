using System;
using System.Linq;
using System.Reflection;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ScriptSystem.Structures;

namespace SER.Testing;

public abstract class Test
{
    public static readonly Test[] Tests = Assembly.GetExecutingAssembly().GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && typeof(Test).IsAssignableFrom(t))
        .Select(t => Activator.CreateInstance(t) as Test)
        .Cast<Test>()
        .ToArray();
    
    protected abstract string Content { get; }

    protected abstract Type[][] TargetTokens { get; }
    
    public static Result Run()
    {
        foreach (var test in Tests.Select(t => Activator.CreateInstance(t.GetType()) as Test).Cast<Test>())
        {
            var scr = new Script
            {
                Name = "Test",
                Content = test.Content,
                Executor = ScriptExecutor.Get()
            };

            if (scr.DefineLines().HasErrored(out var err) || 
                scr.SliceLines().HasErrored(out err) ||
                scr.TokenizeLines().HasErrored(out err)
               )
            {
                return err;
            }

            foreach (var tuple in test.TargetTokens.Zip(scr.Lines, (expectedTypes, line) => (expectedTypes, line)))
            {
                var actualTokens = tuple.line.Tokens.Select(t => t.GetType()).ToArray();
                
                string mainError = $"Error in line {tuple.line.LineNumber} from test {test.GetType().Name}\n";
                string attachedInfo = $"\n" +
                                      $"Line: {tuple.line.RawRepresentation}\n" +
                                      $"\n" +
                                      $"Expected: [{tuple.expectedTypes.Select(t => t.Name).JoinStrings(", ")}] ({tuple.expectedTypes.Length})\n" +
                                      $"Actual: [{actualTokens.Select(t => t.Name).JoinStrings(", ")}] ({actualTokens.Length})\n" +
                                      $"\n" +
                                      $"Script lines: {scr.Lines.Length} | " +
                                      $"Script slices: {scr.Lines.Sum(s => s.Slices.Length)} | " +
                                      $"Script tokens: {scr.Lines.Sum(t => t.Tokens.Length)}";

                if (actualTokens.Length != tuple.expectedTypes.Length)
                {
                    return mainError + 
                           $"Length of expected tokens ({tuple.expectedTypes.Length}) does not match actual tokens ({actualTokens.Length})."
                           + attachedInfo;
                }
                
                for (var index = 0; index < tuple.expectedTypes.Length; index++)
                {
                    var expectedType = tuple.expectedTypes[index];
                    var actualType = actualTokens[index];

                    if (expectedType.IsAssignableFrom(actualType))
                    {
                        continue;
                    }
                    
                    return mainError + 
                           $"Expected token #{index+1} to be of type {expectedType.Name}, but was {actualType.Name}."
                           + attachedInfo;
                }
            }
        }

        return true;
    }
}