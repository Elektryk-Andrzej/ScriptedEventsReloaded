using System.Collections.Generic;
using System.Linq;
using MEC;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.MethodSystem.ArgumentSystem;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.ScriptSystem;

namespace SER.MethodSystem.BaseMethods;

/// <summary>
///     Represents a base method.
/// </summary>
/// <remarks>
///     Do NOT use this to define a SER method, as it has no Execute() method.
///     Use <see cref="Method" /> or <see cref="YieldingMethod" />.
/// </remarks>
public abstract class BaseMethod
{
    protected BaseMethod()
    {
        var type = GetType();
        Subgroup = type.Namespace?.Split('.').LastOrDefault()?.Replace("Methods", "") ?? "Unknown";
        
        var name = type.Name;
        if (!name.EndsWith("Method"))
        {
            throw new DeveloperFuckupException($"Method class name '{name}' must end with 'Method'.");
        }
        
        Name = name.Substring(0, name.Length - "Method".Length);
        Args = new(this);
    }

    public readonly string Name;
    
    public abstract string Description { get; }
    
    public abstract BaseMethodArgument[] ExpectedArguments { get; }
    
    public ProvidedArguments Args { get; }
    
    public Script Script { get; set; } = null!;

    public readonly string Subgroup;
    
    private readonly List<CoroutineHandle> _coroutines = [];
    
    protected CoroutineHandle RunCoroutine(IEnumerator<float> coro)
    {
        var handle = coro.Run(Script);
        _coroutines.Add(handle);
        return handle;
    }

    public void Terminate()
    {
        _coroutines.ForEach(x => Timing.KillCoroutines(x));
    }
}