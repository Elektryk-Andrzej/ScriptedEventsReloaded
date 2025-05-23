using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LabApi.Features.Console;
using MEC;
using SER.ScriptSystem.ContextSystem.Extensions;
using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.MethodSystem.Exceptions;
using SER.Plugin;
using SER.ScriptSystem.ContextSystem;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.Structures;
using SER.ScriptSystem.TokenSystem.Tokens;
using SER.VariableSystem;
using SER.VariableSystem.Structures;

namespace SER.ScriptSystem;

public class Script
{
    public required string Name { get; init; }
    public required string Content { get; init; }
    public List<ScriptLine> Tokens = [];
    public List<BaseContext> Contexts = [];
    public int CurrentLine { get; set; } = 0;
    public bool IsRunning = false;

    private readonly HashSet<LiteralVariable> _localLiteralVariables = [];
    private readonly HashSet<PlayerVariable> _localPlayerVariables = [];
    private CoroutineHandle _scriptCoroutine;
    private List<CoroutineHandle> _otherCoroutines = [];
    private bool? _isEventAllowed;

    public static TryGet<Script> CreateByScriptName(string scriptName)
    {
        var name = Path.GetFileNameWithoutExtension(scriptName);
        
        if (!FileSystem.DoesScriptExist(name, out var path))
        {
            return $"Script '{name}' does not exist in the SER folder or is inaccessible.";
        }

        return new Script
        {
            Name = name,
            Content = File.ReadAllText(path)
        };
    }
    
    public static TryGet<Script> CreateByPath(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        
        if (!FileSystem.DoesScriptExist(path))
        {
            return $"Script '{name}' does not exist in the SER folder or is inaccessible.";
        }

        return new Script
        {
            Name = name,
            Content = File.ReadAllText(path)
        };
    }
    
    public static Script CreateByVerifiedPath(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        return new Script
        {
            Name = name,
            Content = File.ReadAllText(path)
        };
    }

    public List<ScriptLine> GetFlagLines()
    {
        CacheTokens();

        return Tokens.Where(l => l.Tokens.FirstOrDefault() is FlagToken).ToList();
    }

    public void AddLocalLiteralVariable(LiteralVariable variable)
    {
        RemoveLocalLiteralVariable(variable);
        _localLiteralVariables.Add(variable);
    }

    public void RemoveLocalLiteralVariable(LiteralVariable variable)
    {
        _localLiteralVariables.RemoveWhere(scrVar => scrVar.Name == variable.Name);
    }

    public void AddLocalPlayerVariable(PlayerVariable variable)
    {
        RemoveLocalPlayerVariable(variable);
        _localPlayerVariables.Add(variable);
    }

    public void RemoveLocalPlayerVariable(PlayerVariable variable)
    {
        _localPlayerVariables.RemoveWhere(scrVar => scrVar.Name == variable.Name);
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
                    throw new DeveloperFuckupException();
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
        
        Plugin.MainPlugin.RunningScripts.Add(this);
        IsRunning = true;
        _scriptCoroutine = InternalExecute().Run(this, _ => _scriptCoroutine.Kill());
        Logger.Info("returning value");
        return _isEventAllowed;
    }

    public void Stop()
    {
        IsRunning = false;
        Plugin.MainPlugin.RunningScripts.Remove(this);
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

            var handle = context.ExecuteBaseContext().Run(this);
            
            while (handle.IsRunning)
            {
                yield return 0f;
            }
        }

        Plugin.MainPlugin.RunningScripts.Remove(this);
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

        return $"There is no literal variable named '{name}'.";
    }
}