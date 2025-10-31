using System.IO;
using System.Linq;
using System.Reflection;
using LabApi.Features.Console;
using LabApi.Loader.Features.Paths;
using SER.Examples;
using SER.FlagSystem;
using SER.Helpers;
using SER.Helpers.Extensions;
using SER.ScriptSystem;
using SER.ScriptSystem.Structures;

namespace SER.Plugin;

public static class FileSystem
{
    public static readonly string DirPath = Path.Combine(PathManager.Configs.FullName, "Scripted Events Reloaded");
    public static string[] RegisteredScriptPaths = [];

    public static void UpdateScriptPathCollection()
    {
        RegisteredScriptPaths = Directory
            .GetFiles(DirPath, "*.txt", SearchOption.AllDirectories)
            // ignore files with a pound sign at the start
            .Where(path => Path.GetFileName(path).FirstOrDefault() != '#')
            .ToArray();
        
        //Log.Signal(RegisteredScriptPaths.JoinStrings(" "));
        
        var duplicates = RegisteredScriptPaths
            .Select(Path.GetFileNameWithoutExtension)
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => (g.Key, g.Count()))
            .ToList();
        
        if (!duplicates.Any()) return;
        Logger.Error(
            $"There are {string.Join(", ", duplicates.Select(d => $"{d.Item2} scripts named '{d.Key}'"))}\n" +
            $"Please rename them to avoid conflicts."
        );
        
        RegisteredScriptPaths = RegisteredScriptPaths
            .Where(path => !duplicates.Select(d => d.Key).Contains(Path.GetFileNameWithoutExtension(path)))
            .ToArray();
    }
    
    public static void Initialize()
    {
        if (!Directory.Exists(DirPath))
        {
            Directory.CreateDirectory(DirPath);
            return;
        }
        
        UpdateScriptPathCollection();
        ScriptFlagHandler.Clear();
        
        foreach (var scriptPath in RegisteredScriptPaths)
        {
            var scriptName = Path.GetFileNameWithoutExtension(scriptPath);

            var lines = Script.CreateByVerifiedPath(scriptPath, ServerConsoleExecutor.Instance).GetFlagLines();
            if (lines.IsEmpty())
            {
                continue;
            }
            
            ScriptFlagHandler.RegisterScript(lines, scriptName);
        }
    }
    
    public static bool DoesScriptExist(string scriptName, out string path)
    {
        UpdateScriptPathCollection();
        
        path = RegisteredScriptPaths.FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == scriptName) ?? "";
        return !string.IsNullOrEmpty(path);
    }
    
    public static bool DoesScriptExist(string path)
    {
        UpdateScriptPathCollection();
        
        return RegisteredScriptPaths.Any(p => p == path);
    }

    public static void GenerateExamples()
    {
        var examples = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(IExample).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(t => t.CreateInstance<IExample>());

        var dir = Directory.CreateDirectory(Path.Combine(DirPath, "Example Scripts"));

        foreach (var example in examples)
        {
            var path = Path.Combine(dir.FullName, $"{example.Name}.txt");
            Log.Signal($"created path {path}");
            using var sw = File.CreateText(path);
            sw.Write(example.Content);
        }
    }
}