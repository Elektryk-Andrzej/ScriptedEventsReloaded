using System.IO;
using System.Linq;
using LabApi.Features.Console;
using SER.ScriptSystem;
using SER.ScriptSystem.FlagSystem;

namespace SER.Plugin;

public static class FileSystem
{
    public static readonly string DirPath = Path.Combine(LabApi.Loader.Features.Paths.PathManager.Configs.FullName, "Scripted Events Reloaded");
    public static string[] RegisteredScriptPaths = [];

    public static void UpdateScriptPathCollection()
    {
        RegisteredScriptPaths = Directory.GetFiles(DirPath, "*.txt", SearchOption.AllDirectories);
        
        var duplicates = RegisteredScriptPaths
            .Select(Path.GetFileNameWithoutExtension)
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => (g.Key, g.Count()))
            .ToList();

        
        if (!duplicates.Any()) return;
        Logger.Error(
            $"There are {string.Join(", ", duplicates.Select(d => $"{d.Item2} scripts named '{d.Key}'"))}\n" +
            $"Please rename them to avoid conflicts.");
        
        RegisteredScriptPaths = RegisteredScriptPaths
            .Where(path => !duplicates.Select(d => d.Key).Contains(Path.GetFileNameWithoutExtension(path)))
            .ToArray();
    }
    
    public static void Initalize()
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

            var lines = Script.CreateByVerifiedPath(scriptPath).GetFlagLines();
            
            ScriptFlagHandler.RegisterScript(lines, scriptName);
        }
        
        ScriptFlagHandler.RegisterCommands();
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
}