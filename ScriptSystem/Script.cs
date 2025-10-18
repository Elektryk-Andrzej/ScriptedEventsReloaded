using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using SER.ContextSystem;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Extensions;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.Plugin;
using SER.ScriptSystem.Structures;
using SER.TokenSystem;
using SER.TokenSystem.Structures;
using SER.TokenSystem.Tokens;
using SER.VariableSystem;
using SER.VariableSystem.Variables;

namespace SER.ScriptSystem;

public class Script
{
    public required string Name { get; init; }
    
    public required string Content { get; init; }
    
    public required ScriptExecutor Executor
    {
        get;
        init
        {
            if (value is RemoteAdminExecutor { Sender: { } sender } && Player.TryGet(sender, out var player))
            {
                AddLocalPlayerVariable(new("sender", [player]));
            }

            field = value;
        }
    }
    
    public Line[] Lines = [];
    public Context[] Contexts = [];
    
    public uint CurrentLine { get; set; } = 0;
    
    public bool IsRunning => RunningScripts.Contains(this);

    private static readonly List<Script> RunningScripts = [];
    private readonly HashSet<LiteralVariable> _localLiteralVariables = [];
    private readonly HashSet<PlayerVariable> _localPlayerVariables = [];
    private CoroutineHandle _scriptCoroutine;
    private bool? _isEventAllowed;

    public void Reply(string message)
    {
        Executor.Reply(message, this);
    }
    
    public void Warn(string message)
    {
        Executor.Warn(message, this);
    }
    
    public void Error(string message)
    {
        Executor.Error(message, this);
        Stop();
    }

    public static TryGet<Script> CreateByScriptName(string dirtyName, ScriptExecutor executor)
    {
        var name = Path.GetFileNameWithoutExtension(dirtyName);
        if (!FileSystem.DoesScriptExist(name, out var path))
        {
            return $"Script '{name}' does not exist in the SER folder or is inaccessible.";
        }

        return new Script
        {
            Name = name,
            Content = File.ReadAllText(path),
            Executor = executor
        };
    }
    
    public static TryGet<Script> CreateByPath(string path, ScriptExecutor executor)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        
        if (!FileSystem.DoesScriptExist(path))
        {
            return $"Script '{name}' does not exist in the SER folder or is inaccessible.";
        }

        return new Script
        {
            Name = name,
            Content = File.ReadAllText(path),
            Executor = executor
        };
    }
    
    public static Script CreateByVerifiedPath(string path, ScriptExecutor executor)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        return new Script
        {
            Name = name,
            Content = File.ReadAllText(path),
            Executor = executor
        };
    }

    public static int StopAll()
    {
        var count = RunningScripts.Count;
        foreach (var script in new List<Script>(RunningScripts))
        {
            script.Stop();
        }

        return count;
    }
    
    public static int StopByName(string name)
    {
        var matches = new List<Script>(RunningScripts)
            .Where(scr => string.Equals(scr.Name, name, StringComparison.CurrentCultureIgnoreCase))
            .ToArray();
        
        matches.ForEachItem(scr => scr.Stop());
        return matches.Length;
    }

    public List<Line> GetFlagLines()
    {
        DefineLines();
        SliceLines();
        TokenizeLines();
        return Lines.Where(l => l.Tokens.FirstOrDefault() is FlagToken or FlagArgumentToken).ToList();
    }

    public void AddLocalLiteralVariable(LiteralVariable variable)
    {
        Log.Debug($"Added variable {variable.Name} to script {Name}");
        RemoveLocalLiteralVariable(variable.Name);
        _localLiteralVariables.Add(variable);
    }

    public void RemoveLocalLiteralVariable(string name)
    {
        _localLiteralVariables.RemoveWhere(scrVar => scrVar.Name == name);
    }

    public void AddLocalPlayerVariable(PlayerVariable variable)
    {
        Log.Debug($"Added player variable {variable.Name} to script {Name}");
        RemoveLocalPlayerVariable(variable.Name);
        _localPlayerVariables.Add(variable);
    }

    public void RemoveLocalPlayerVariable(string name)
    {
        _localPlayerVariables.RemoveWhere(scrVar => scrVar.Name == name);
    }

    public void AddVariables(params IVariable[] variables)
    {
        foreach (var variable in variables)
        {
            switch (variable)
            {
                case LiteralVariable literalVariable:
                    AddLocalLiteralVariable(literalVariable);
                    break;
                case PlayerVariable playerVariable:
                    AddLocalPlayerVariable(playerVariable);
                    break;
                default:
                    throw new AndrzejFuckedUpException();
            }
        }
    }

    /// <summary>
    /// Executes the script.
    /// </summary>
    public void Run()
    {
        RunForEvent();
    }

    /// <summary>
    /// Executes the script.
    /// </summary>
    /// <returns>Returns a boolean indicating whether the event is allowed.</returns>
    public bool? RunForEvent()
    {
        if (string.IsNullOrWhiteSpace(Content))
        {
            return null;
        }
        
        RunningScripts.Add(this);
        _scriptCoroutine = InternalExecute().Run(this, _ => _scriptCoroutine.Kill());
        return _isEventAllowed;
    }

    public void Stop()
    {
        RunningScripts.Remove(this);
        _scriptCoroutine.Kill();
        Logger.Info($"Script {Name} was stopped");
    }

    public void SendControlMessage(ScriptControlMessage msg)
    {
        if (msg == ScriptControlMessage.EventNotAllowed)
        {
            _isEventAllowed = false;
        }
    }

    public Result DefineLines()
    {
        if (Tokenizer.GetInfoFromMultipleLines(Content).HasErrored(out var err, out var info))
        {
            return "Defining script lines failed." + err;
        }
        
        Log.Debug($"Script {Name} defines {info.Length} lines");
        Lines = info;
        return true;
    }
    
    public Result SliceLines()
    {
        foreach (var line in Lines)
        {
            if (Tokenizer.SliceLine(line).HasErrored(out var error))
            {
                Result mainErr = $"Processing line {line.LineNumber} has failed.";
                return error;
            }
        }
        
        Log.Debug($"Script {Name} sliced {Lines.Length} lines into {Lines.Sum(l => l.Slices.Length)} slices");
        return true;
    }

    public Result TokenizeLines()
    {
        foreach (var line in Lines)
        {
            Tokenizer.TokenizeLine(line, this);
        }

        Log.Debug($"Script {Name} tokenized {Lines.Length} lines into {Lines.Sum(l => l.Tokens.Length)} tokens");
        return true;
    }
    
    private Result ContextLines()
    {
        if (Contexter.ContextLines(Lines, this).HasErrored(out var err, out var contexts))
        {
            return err;
        }
        
        Contexts = contexts;
        return true;
    }

    private IEnumerator<float> InternalExecute()
    {
        if (DefineLines().HasErrored(out var err) || 
            SliceLines().HasErrored(out err) ||
            TokenizeLines().HasErrored(out err) || 
            ContextLines().HasErrored(out err))
        {
            throw new ScriptErrorException(err);
        }
        
        foreach (var context in Contexts)
        {
            if (!IsRunning)
            {
                break;
            }

            var handle = context.ExecuteBaseContext();
            while (handle.MoveNext())
            {
                if (!IsRunning)
                {
                    break;
                }
                
                yield return handle.Current;
            }
        }

        RunningScripts.Remove(this);
    }

    public TryGet<PlayerVariable> TryGetPlayerVariable(string name)
    {
        var localPlrVar = _localPlayerVariables.FirstOrDefault(
            pv => pv.Name == name);

        if (localPlrVar != null)
        {
            return localPlrVar;
        }

        var globalPlrVar = PlayerVariableIndex.GlobalPlayerVariables
            .FirstOrDefault(v => v.Name == name);
        if (globalPlrVar == null)
        {
            return $"There is no player variable named '@{name}'.";
        }
        
        return globalPlrVar;
    }
    
    public TryGet<PlayerVariable> TryGetPlayerVariable(PlayerVariableToken token)
    {
        return TryGetPlayerVariable(token.Name);
    }

    public TryGet<LiteralVariable> TryGetLiteralVariable(string name)
    {
        var localPlrVar = _localLiteralVariables.FirstOrDefault(v => v.Name == name);
        Log.Debug($"Fetching literal variable '{name}', there currently exist {_localLiteralVariables.Count} " +
                  $"variables: {_localLiteralVariables.Select(v => v.Name).JoinStrings(", ")}");
        if (localPlrVar != null)
        {
            return localPlrVar;
        }
        
        var globalVar = LiteralVariableIndex.GlobalLiteralVariables
            .FirstOrDefault(v => v.Name == name);
        if (globalVar != null)
        {
            return globalVar;
        }

        return $"There is no literal variable called {name}.";
    }
    
    public TryGet<LiteralVariable> TryGetLiteralVariable(LiteralVariableToken token)
    {
        return TryGetLiteralVariable(token.Name);
    }
}