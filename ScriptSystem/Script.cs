using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using SER.ScriptSystem.ContextSystem.Extensions;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultStructure;
using SER.Plugin;
using SER.ScriptSystem.ContextSystem;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.Structures;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.Structures;
using SER.ScriptSystem.TokenSystem.Tokens;
using SER.ScriptSystem.TokenSystem.Tokens.LiteralVariables;
using SER.VariableSystem;
using SER.VariableSystem.Structures;

namespace SER.ScriptSystem;

public class Script
{
    public required string Name { get; init; }
    public required string Content { get; init; }

    private readonly ScriptExecutor? _executor;
    public required ScriptExecutor Executor
    {
        get => _executor!;
        init
        {
            if (value is RemoteAdminExecutor { Sender: { } sender } && Player.TryGet(sender, out var player))
            {
                AddLocalPlayerVariable(new("sender", [player]));
            }

            _executor = value;
        }
    }
    
    public List<ScriptLine> Tokens = [];
    public List<Context> Contexts = [];
    public int CurrentLine { get; set; } = 0;
    public bool IsRunning => RunningScripts.Contains(this);

    private static readonly List<Script> RunningScripts = [];
    private readonly HashSet<LiteralVariable> _localLiteralVariables = [];
    private readonly HashSet<PlayerVariable> _localPlayerVariables = [];
    private CoroutineHandle _scriptCoroutine;
    private bool? _isEventAllowed;

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

    public List<ScriptLine> GetFlagLines()
    {
        CacheTokens();

        return Tokens.Where(l => l.Tokens.FirstOrDefault() is FlagToken or FlagArgumentToken).ToList();
    }

    public void AddLocalLiteralVariable(LiteralVariable variable)
    {
        RemoveLocalLiteralVariable(variable.Name);
        _localLiteralVariables.Add(variable);
    }

    public void RemoveLocalLiteralVariable(string name)
    {
        _localLiteralVariables.RemoveWhere(scrVar => scrVar.Name == name);
    }

    public void AddLocalPlayerVariable(PlayerVariable variable)
    {
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

    private Result CacheTokens()
    {
        try
        {
            new Tokenizer(this).GetAllFileTokens();
            return true;
        }
        catch (Exception e)
        {
            return e.ToString();
        }
    }
    
    private Result CacheContexts()
    {
        try
        {
            if (new Contexter(this).LinkAllTokens(Tokens)
                .HasErrored(out var err, out var val))
            {
                return err;
            }

            Contexts = val;
            return true;
        }
        catch (Exception e)
        {
            return e.ToString();
        }
    }

    private IEnumerator<float> InternalExecute()
    {
        if (CacheTokens().HasErrored(out var tokenError))
        {
            Log.Error(this, tokenError);
            yield break;
        }

        if (CacheContexts().HasErrored(out var contextError))
        {
            Log.Error(this, contextError);
            yield break;
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

    public TryGet<LiteralVariable> TryGetLiteralVariable(string name)
    {
        var localPlrVar = _localLiteralVariables.FirstOrDefault(v => v.Name == name);
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

        return $"There is no literal variable named '{{{name}}}'.";
    }
    
    public TryGet<LiteralVariable> TryGetLiteralVariable(LiteralVariableToken token)
    {
        var localPlrVar = _localLiteralVariables.FirstOrDefault(v => v.Name == token.ValueWithoutBrackets);
        if (localPlrVar != null)
        {
            return localPlrVar;
        }
        
        var globalVar = LiteralVariableIndex.GlobalLiteralVariables
            .FirstOrDefault(v => v.Name == token.ValueWithoutBrackets);
        if (globalVar != null)
        {
            return globalVar;
        }

        return $"There is no literal variable named '{token.RawRepresentation}'.";
    }
}